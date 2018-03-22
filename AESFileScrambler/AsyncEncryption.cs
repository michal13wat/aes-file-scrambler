﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace AESFileScrambler
{
    class AsyncEncryption
    {
        public BackgroundWorker backgroundWorker;
        public AsyncEncryption()
        {
            backgroundWorker = new BackgroundWorker();
            backgroundWorker.WorkerReportsProgress = true;
            backgroundWorker.WorkerSupportsCancellation = true;
            backgroundWorker.DoWork += EncyrptAsyncBackgroundWorker;
            backgroundWorker.RunWorkerCompleted += EncryptAsyncCompleted;
        }

        void EncryptAsyncCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            backgroundWorker.DoWork -= EncyrptAsyncBackgroundWorker;
            backgroundWorker.RunWorkerCompleted -= EncryptAsyncCompleted;
            backgroundWorker.ProgressChanged -= 
                (System.Windows.Application.Current.MainWindow as MainWindow)
                .UpdateEncProgressBar;
            MessageBox.Show("Succes!\nFile is encprypted.");
        }

        void EncyrptAsyncBackgroundWorker(object sender, DoWorkEventArgs e)
        {
            DataForEnc data = (DataForEnc)e.Argument;

            byte[] saltBytes = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };
            string cryptFile = data.OutputFile;
            FileStream fsCrypt = new FileStream(cryptFile, FileMode.Create);

            RijndaelManaged AES = new RijndaelManaged();

            AES.KeySize = keySize;
            AES.BlockSize = blockSize;


            var key = new Rfc2898DeriveBytes(data.PasswordBytes, saltBytes, 1000);
            AES.Key = key.GetBytes(AES.KeySize / 8);
            AES.IV = key.GetBytes(AES.BlockSize / 8);
            AES.Padding = PaddingMode.Zeros;

            AES.Mode = data.CipherMode;

            CryptoStream cs = new CryptoStream(fsCrypt,
                 AES.CreateEncryptor(),
                CryptoStreamMode.Write);

            FileStream fsIn = new FileStream(data.InputFile, FileMode.Open);

            int encryptedData;
            long lenStream = fsIn.Length;
            //while ((encryptedData = fsIn.ReadByte()) != -1)
            //    cs.WriteByte((byte)encryptedData);
            int prevVal = 0;
            for (long i = 0; (encryptedData = fsIn.ReadByte()) != -1; i++)
            {
                cs.WriteByte((byte)encryptedData);
                if (prevVal != unchecked((int)(i * 100 / lenStream) )) {
                    prevVal = unchecked((int)(i * 100 / lenStream));
                    backgroundWorker.ReportProgress(prevVal);
                }
            }
            backgroundWorker.ReportProgress(100);


            fsIn.Close();
            cs.Close();
            fsCrypt.Close();

            //------------------------------------------------------

            //for (int i = 0; i < 100; i++) {
            //    //Do Something
            //    Thread.Sleep(20);
            //    backgroundWorker.ReportProgress(i);
            //}

            ////Do Something
            //Thread.Sleep(2000);
            //backgroundWorker.ReportProgress(70);

            ////Do Something
            //Thread.Sleep(2000);
            //backgroundWorker.ReportProgress(100);
        }

        private static int keySize = 128;
        private static int blockSize = 128;
    }
}