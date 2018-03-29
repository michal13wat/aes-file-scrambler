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

            tbEncInFile.Text = encInFile;
            tbEncOutFile.Text = encOutFile;
            tbDecInFile.Text = decInFile;
            tbDecOutFile.Text = decOutFile;

            tablePreparedUsers.Columns.Add(_gridViewUser.Header.ToString());
            tablePreparedUsers.Columns.Add(_gridViewPasswd.Header.ToString());

            dataForDec.InputFile = decInFile;
            XmlTextReaderWriter reader = new XmlTextReaderWriter(dataForDec);
            dataForDec = reader.ReadXml();

            try{
                cbUsers.ItemsSource = dataForDec.RSA_UsersKeys.Keys;
            }
            catch { }

            secretPrimeNumber = PrimeNumberGenerator.genpr2(128);
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

            if(usersPasswords.ContainsKey(user))
                MessageBox.Show("User already exists. Try with another name!");
            else{
                
                usersPasswords.Add(user, mySHA256.ComputeHash(Encoding.ASCII.GetBytes(passwd)));

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

        private CipherMode mapEncModeStringToEnum(string modeName) {
            CipherMode encMode;

            string encModeString = modeName;
            switch (encModeString)
            {
                case "CBC": encMode = CipherMode.CBC; break;
                case "CFB": encMode = CipherMode.CFB; break;
                case "EBC": encMode = CipherMode.ECB; break;
                case "OFB": encMode = CipherMode.OFB; break;
                default: encMode = CipherMode.CBC; break;
            }
            return encMode;
        }

        private void btnEncryptClik(object sender, RoutedEventArgs e){
            byte[] passwdHash;

            //usersPasswords.TryGetValue("John", out passwdHash)
            if (true) {

                DataForEnc data = new DataForEnc();
                data.InputFile = encInFile;
                data.OutputFile = encOutFile;
                data.AES_KeyBytes = secretPrimeNumber.ToByteArray();  // mySHA256.ComputeHash(secretPrimeNumber);
                data.CipherMode = mapEncModeStringToEnum(cbEncMode.Text);
                data.StringCipherMode = cbEncMode.Text;
                data.KeySize = 128;
                data.BlockSize = 128;
                data.RSA_UsersKeys = this.usersPasswords;

                AES_AsyncEncryptionFile asyncEnc = new AES_AsyncEncryptionFile();
                asyncEnc.backgroundWorker.ProgressChanged += updateEncProgressBar;
                asyncEnc.backgroundWorker.RunWorkerAsync(data);

                //Encryption.AES_Encrypt(encInFile, encOutFile, mySHA256.ComputeHash(passwdHash),
                //mapEncModeStringToEnum(cbEncMode.Text));
            }
        }
        
        private void btnDecryptClick(object sender, RoutedEventArgs e){
            byte[] passwdHash;

            if (dataForDec.RSA_UsersKeys.TryGetValue(cbUsers.Text.ToString(), out passwdHash)){

                dataForDec.InputFile = decInFile;
                dataForDec.OutputFile = decOutFile;
                dataForDec.AES_KeyBytes = secretPrimeNumber.ToByteArray();  //mySHA256.ComputeHash(passwdHash);

                // TODO - CipeherMode musi być odczytywany z pliku!!!
                //dataForDec.CipherMode = CipherMode.CBC; //mapEncModeStringToEnum(cbEncMode.Text);

                AES_AsyncDecryptionFile asyncDec = new AES_AsyncDecryptionFile();
                asyncDec.backgroundWorker.ProgressChanged += updateDecProgressBar;
                asyncDec.backgroundWorker.RunWorkerAsync(dataForDec);
            }
        }

        private void btnCreateRecepients_Click(object sender, RoutedEventArgs e){

        }

        private void btnEncInFile_Click(object sender, RoutedEventArgs e){

            bool? temp = file.ShowDialog();
            if (temp.HasValue ? temp.Value : false){
                encInFile = file.FileName;
                tbEncInFile.Text = encInFile;
            }
        }

        private void btnEndOutFile_Click(object sender, RoutedEventArgs e){

            bool? temp = file.ShowDialog();
            if (temp.HasValue ? temp.Value : false)
            {
                encOutFile = file.FileName;
                tbEncOutFile.Text = encOutFile;
            }
        }

        private void btnDecInFile_Click(object sender, RoutedEventArgs e){

            bool? temp = file.ShowDialog();
            if (temp.HasValue ? temp.Value : false)
            {
                decInFile = file.FileName;
                tbDecInFile.Text = decInFile;

                dataForDec.InputFile = decInFile;
                XmlTextReaderWriter reader = new XmlTextReaderWriter(dataForDec);
                // TODO - zapisać to do DataForDecrypted i użytkowników wrzucić na
                // tą listę wyboru
                dataForDec = reader.ReadXml();
                cbUsers.ItemsSource = dataForDec.RSA_UsersKeys.Keys;
            }
        }

        private void btnDecOutFile_Click(object sender, RoutedEventArgs e){

            bool? temp = file.ShowDialog();
            if (temp.HasValue ? temp.Value : false)
            {
                decOutFile = file.FileName;
                tbDecOutFile.Text = decOutFile;
            }
        }

        private Dictionary<string, byte[]> usersPasswords 
            = new Dictionary<string, byte[]>();

        private DataTable tablePreparedUsers = new DataTable();
        private SHA256 mySHA256 = SHA256.Create();
        private string encInFile = "E:\\mojfolder\\Studia\\semestr_6\\BSK\\Projekty\\projekt1\\inExFiles\\3.zip";
        private string encOutFile = "E:\\mojfolder\\Studia\\semestr_6\\BSK\\Projekty\\projekt1\\encryptedFile";
        private string decInFile = "E:\\mojfolder\\Studia\\semestr_6\\BSK\\Projekty\\projekt1\\encryptedFile";
        private string decOutFile = "E:\\mojfolder\\Studia\\semestr_6\\BSK\\Projekty\\projekt1\\outExFiles\\decryptedFile.zip";

        private OpenFileDialog file = new OpenFileDialog();

        private CommonDataEncDec dataForDec = new DataForDec();
        private Org.BouncyCastle.Math.BigInteger secretPrimeNumber = new Org.BouncyCastle.Math.BigInteger("0");
        //private Org.BouncyCastle.Math.BigInteger encryptedPrimeNumber = new Org.BouncyCastle.Math.BigInteger("0");
        //private Org.BouncyCastle.Math.BigInteger decryptedPrimeNumber = new Org.BouncyCastle.Math.BigInteger("0");
    }
}