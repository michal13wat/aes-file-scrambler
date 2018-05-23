using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Security.Cryptography;
using Microsoft.Win32;
using System.ComponentModel;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;

namespace AESFileScrambler
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            tbEncInFile.Text = AES_Configuration.encInFile;
            tbEncOutFile.Text = AES_Configuration.encOutFile;
            tbDecInFile.Text = AES_Configuration.decInFile;
            tbDecOutFile.Text = AES_Configuration.decOutFile;

            tablePreparedUsers.Columns.Add(_gridViewUser.Header.ToString());
            tablePreparedUsers.Columns.Add(_gridViewPasswd.Header.ToString());

            dataForDec.InputFile = AES_Configuration.decInFile;
            XmlTextReaderWriter reader = new XmlTextReaderWriter(dataForDec);
            dataForDec = reader.ReadXml();

            try{
                cbUsers.ItemsSource = dataForDec.UsersCollection.Keys;
            }
            catch { }

            //AES_Configuration.secretPrimeNumber = PrimeNumberGenerator.genpr2(128);
        }

        public void updateEncProgressBar(object sender, ProgressChangedEventArgs e)
        {
            // Upadte the Progress bar
            pbEnc.Value = e.ProgressPercentage;
            lbEncProgress.Content = e.ProgressPercentage + "%";
        }

        public void updateDecProgressBar(object sender, ProgressChangedEventArgs e)
        {
            // Upadte the Progress bar
            pbDec.Value = e.ProgressPercentage;
            lbDecProgress.Content = e.ProgressPercentage + "%";
        }


        /// <summary>
        /// This method show in table masked password and
        /// create HASH of this password.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonAddUserClick(object sender, RoutedEventArgs e)
        {
            string user = tbUserName.Text;
            string passwd = pbPassword.Password.ToString();

            if(dataForEnc.UsersCollection.ContainsKey(user))
                MessageBox.Show("User already exists. Try with another name!");
            else{

                dataForEnc.UsersCollection.Add(user, new UserData() {
                    Name = user,
                    Passwd = mySHA256.ComputeHash(Encoding.ASCII.GetBytes(passwd))
                });

                tablePreparedUsers.Rows.Add(user, replaceStringOnStars(passwd));
                tablePreparedUsers.AcceptChanges();

                loadTable(tablePreparedUsers);
            }

            tbUserName.Clear();
            pbPassword.Clear();
        }

        private string replaceStringOnStars(string passwd) {
            string maskedPasswd = "";
            for (int i = 0; i < passwd.Length; i++)
                maskedPasswd += "*";
            return maskedPasswd;
        }

        public void loadTable(DataTable dt)
        {
            _gridView.Columns.Clear();

            _listView.DataContext = dt;


            Binding bind = new Binding();
            _listView.SetBinding(ItemsControl.ItemsSourceProperty, bind);

            foreach (var colum in dt.Columns)
            {
                DataColumn dc = (DataColumn)colum;
                GridViewColumn column = new GridViewColumn();
                column.DisplayMemberBinding = new Binding(dc.ColumnName);
                column.Header = dc.ColumnName;
                _gridView.Columns.Add(column);
            }
        }

        private void btnEncryptClik(object sender, RoutedEventArgs e){

            dataForEnc.InputFile = AES_Configuration.encInFile;
            dataForEnc.OutputFile = AES_Configuration.encOutFile;

            try{
                dataForEnc.AES_KeyBytes = dataForEnc.UsersCollection.First().Value.PlainSesKey;
            }
            catch {
                MessageBox.Show("Error: Before encrypt file you have to add minimum one recepient!");
                return;
            }

            dataForEnc.StringCipherMode = cbEncMode.Text;
            dataForEnc.KeySize = 128;
            dataForEnc.BlockSize = 128;

            XmlTextReaderWriter writer = new XmlTextReaderWriter(dataForEnc);
            writer.WriteToXml();

            AES_AsyncEncryptionFile asyncEnc = new AES_AsyncEncryptionFile();
            asyncEnc.backgroundWorker.ProgressChanged += updateEncProgressBar;
            asyncEnc.backgroundWorker.RunWorkerAsync(dataForEnc);
        }
        
        private void btnDecryptClick(object sender, RoutedEventArgs e){
            UserData userData;
            string key = cbUsers.Text.ToString();
            RSA RSA_Decryptor = new RSA();

            refreshCbUsers();

            if (dataForDec.UsersCollection.TryGetValue(key, out userData)){

                // tutaj zrobić odczytywanie z pliku klucza prywatnego

                try {
                    userData.PrivKey = RSA_Decryptor.readRSAParametersFromFile(RSA_Configuration.keyDirectory + "\\"
                        + key + "_priv.key");
                }
                catch {
                    MessageBox.Show("Error reading private key!");
                    return;
                }


                // tutaj zrobić deszyfrowanie klucza prywatnego

                // tutaj zrobić deszyforwanie klucza sesyjnego
                userData = RSA_Decryptor.DecryptSessionKey(userData);
                dataForDec.UsersCollection[key] = userData;

                dataForDec.InputFile = AES_Configuration.decInFile;
                dataForDec.OutputFile = AES_Configuration.decOutFile;

                dataForDec.AES_KeyBytes = userData.PlainSesKey;

                AES_AsyncDecryptionFile asyncDec = new AES_AsyncDecryptionFile();
                asyncDec.backgroundWorker.ProgressChanged += updateDecProgressBar;
                asyncDec.backgroundWorker.RunWorkerAsync(dataForDec);
            }
        }

        private void btnCreateRecepients_Click(object sender, RoutedEventArgs e){
            RSA RSA_Encryptor = new RSA();
            Org.BouncyCastle.Math.BigInteger secretPrimeNumber = PrimeNumberGenerator.genpr2(128);
            Dictionary<string, UserData> tempDictionary = new Dictionary<string, UserData>();
            UserData tempUserData;

            //if (dataForDec) {
            //    MessageBox.Show("Error before create recepients you have to add them to above list!");
            //    return;
            //}

            foreach (KeyValuePair<string, UserData> u in dataForEnc.UsersCollection)
            {
                u.Value.PlainSesKey = secretPrimeNumber.ToByteArray();

                tempUserData = RSA_Encryptor.EncryptSessionKey(u.Value);
                tempDictionary.Add(u.Key, tempUserData);
            

                // tutaj zrobić szyfrowanie klucza prywatnego


                // zapis kluczy do plików
                RSA_Encryptor.writeRSAParametersToFile(tempUserData.PrivKey,
                    RSA_Configuration.keyDirectory + "\\" + u.Key + "_priv.key");
                RSA_Encryptor.writeRSAParametersToFile(tempUserData.PubKey,
                    RSA_Configuration.keyDirectory + "\\" + u.Key + "_pub.key");

            }

            dataForEnc.UsersCollection = tempDictionary;
        }

        private void btnEncInFile_Click(object sender, RoutedEventArgs e){

            bool? temp = file.ShowDialog();
            if (temp.HasValue ? temp.Value : false){
                AES_Configuration.encInFile = file.FileName;
                tbEncInFile.Text = AES_Configuration.encInFile;
            }
        }

        private void btnEndOutFile_Click(object sender, RoutedEventArgs e){

            bool? temp = file.ShowDialog();
            if (temp.HasValue ? temp.Value : false)
            {
                AES_Configuration.encOutFile = file.FileName;
                tbEncOutFile.Text = AES_Configuration.encOutFile;
            }
        }

        private void btnDecInFile_Click(object sender, RoutedEventArgs e){

            bool? temp = file.ShowDialog();
            if (temp.HasValue ? temp.Value : false)
            {
                AES_Configuration.decInFile = file.FileName;
                tbDecInFile.Text = AES_Configuration.decInFile;

                refreshCbUsers();
            }
        }

        private void refreshCbUsers() {
            dataForDec.InputFile = AES_Configuration.decInFile;
            XmlTextReaderWriter reader = new XmlTextReaderWriter(dataForDec);
            dataForDec = reader.ReadXml();
            cbUsers.ItemsSource = dataForDec.UsersCollection.Keys;
        }

        private void btnDecOutFile_Click(object sender, RoutedEventArgs e){

            bool? temp = file.ShowDialog();
            if (temp.HasValue ? temp.Value : false)
            {
                AES_Configuration.decOutFile = file.FileName;
                tbDecOutFile.Text = AES_Configuration.decOutFile;
            }
        }


        private DataTable tablePreparedUsers = new DataTable();
        private SHA256 mySHA256 = SHA256.Create();

        private OpenFileDialog file = new OpenFileDialog();

        private CommonDataEncDec dataForDec = new DataForDec();
        private CommonDataEncDec dataForEnc = new DataForEnc();
    }
}