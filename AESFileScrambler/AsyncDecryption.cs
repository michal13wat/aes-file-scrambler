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
    class AsyncDecryption
    {
        public BackgroundWorker backgroundWorker;
        public AsyncDecryption()
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
            DataForEnc data = (DataForEnc)e.Argument;

            byte[] saltBytes = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };

            FileStream fsCrypt = new FileStream(data.InputFile, FileMode.Open);

            RijndaelManaged AES = new RijndaelManaged();

            AES.KeySize = keySize;
            AES.BlockSize = blockSize;


            var key = new Rfc2898DeriveBytes(data.PasswordBytes, saltBytes, 1000);
            AES.Key = key.GetBytes(AES.KeySize / 8);
            AES.IV = key.GetBytes(AES.BlockSize / 8);
            AES.Padding = PaddingMode.Zeros;

            AES.Mode = data.CipherMode;

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

        private static int keySize = 128;
        private static int blockSize = 128;
    }
}
