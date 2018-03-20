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

            tablePreparedUsers.Columns.Add(_gridViewUser.Header.ToString());
            tablePreparedUsers.Columns.Add(_gridViewPasswd.Header.ToString());
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

        private void btnEncryptClik(object sender, RoutedEventArgs e){
            byte[] passwdHash;

            if (usersPasswords.TryGetValue("John", out passwdHash)) {
                Encryption.AES_Encrypt("E:\\mojfolder\\Studia\\semestr_6\\BSK\\Projekty\\projekt1\\IMG_20170703_163731.jpg",
                "E:\\mojfolder\\Studia\\semestr_6\\BSK\\Projekty\\projekt1\\encryptedFile", mySHA256.ComputeHash(passwdHash),
                CipherMode.CBC);
            }
        }

        private void btnDecryptClick(object sender, RoutedEventArgs e){
            byte[] passwdHash;

            if (usersPasswords.TryGetValue("John", out passwdHash)){
                Encryption.AES_Decrypt("E:\\mojfolder\\Studia\\semestr_6\\BSK\\Projekty\\projekt1\\encryptedFile",
                "E:\\mojfolder\\Studia\\semestr_6\\BSK\\Projekty\\projekt1\\decryptedFile.jpg", mySHA256.ComputeHash(passwdHash),
                CipherMode.CBC);
            }
        }

        private void btnCreateRecepients_Click(object sender, RoutedEventArgs e){

        }

        private Dictionary<string, byte[]> usersPasswords 
            = new Dictionary<string, byte[]>();

        private DataTable tablePreparedUsers = new DataTable();
        private SHA256 mySHA256 = SHA256.Create();

    }
}
