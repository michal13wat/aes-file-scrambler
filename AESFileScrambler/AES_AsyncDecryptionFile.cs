using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AESFileScrambler
{
    class AES_AsyncDecryptionFile
    {
        public BackgroundWorker backgroundWorker;
        public AES_AsyncDecryptionFile()
        {
            backgroundWorker = new BackgroundWorker();
            backgroundWorker.WorkerReportsProgress = true;
            backgroundWorker.WorkerSupportsCancellation = true;
            backgroundWorker.DoWork += DecyrptAsyncBackgroundWorker;
            backgroundWorker.RunWorkerCompleted += DecryptAsyncCompleted;
        }

        void DecryptAsyncCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            backgroundWorker.DoWork -= DecyrptAsyncBackgroundWorker;
            backgroundWorker.RunWorkerCompleted -= DecryptAsyncCompleted;
            backgroundWorker.ProgressChanged -=
                (System.Windows.Application.Current.MainWindow as MainWindow)
                .updateDecProgressBar;
            MessageBox.Show("Succes!\nFile is decprypted.");
        }

        void DecyrptAsyncBackgroundWorker(object sender, DoWorkEventArgs e)
        {
            DataForDec data = (DataForDec)e.Argument;

            byte[] saltBytes = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };

            FileStream fsCrypt = new FileStream(data.InputFile, FileMode.Open);
            Int32 position = 0;
            using (StreamReader streamReader = new StreamReader(fsCrypt))
            {
                while (!streamReader.EndOfStream)
                {
                    string line = streamReader.ReadLine();
                    position += line.Length;
                    if (line.Equals(delimiter))
                    {
                        // dla jednego użytkownika działało -22
                        fsCrypt.Seek(position - 22, SeekOrigin.Begin);
                        break;
                    }
                }

                RijndaelManaged AES = new RijndaelManaged();

                AES.KeySize = data.KeySize;
                AES.BlockSize = data.BlockSize;


                var key = new Rfc2898DeriveBytes(data.AES_KeyBytes, saltBytes, 1000);
                AES.Key = key.GetBytes(AES.KeySize / 8);
                AES.IV = key.GetBytes(AES.BlockSize / 8);
                AES.Padding = PaddingMode.Zeros;

                // TODO - zmienić to, żeby nie było ustawione na stałe!!!
                AES.Mode = CipherMode.CBC; //data.CipherMode;

                CryptoStream cs = new CryptoStream(fsCrypt,
                    AES.CreateDecryptor(),
                    CryptoStreamMode.Read);

                FileStream fsOut = new FileStream(data.OutputFile, FileMode.Create);

                int encryptedData;
                long lenStream = fsCrypt.Length;

                int prevVal = 0;

                for (long i = 0; (encryptedData = cs.ReadByte()) != -1; i++)
                {
                    fsOut.WriteByte((byte)encryptedData);
                    if (prevVal != unchecked((int)(i * 100 / lenStream)))
                    {
                        prevVal = unchecked((int)(i * 100 / lenStream));
                        backgroundWorker.ReportProgress(prevVal);
                    }
                }
                backgroundWorker.ReportProgress(100);


                fsOut.Close();
                cs.Close();
                fsCrypt.Close();

            }
        }

        private const string delimiter = "=====================================================";
    }
}
