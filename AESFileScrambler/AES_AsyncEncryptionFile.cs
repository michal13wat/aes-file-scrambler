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
using System.Diagnostics;

namespace AESFileScrambler
{
    public class AES_AsyncEncryptionFile : AES_AsyncCommon
    {
        public AES_AsyncEncryptionFile()
        {
            backgroundWorker.DoWork += EncyrptAsyncBackgroundWorker;
            backgroundWorker.RunWorkerCompleted += EncryptAsyncCompleted;
        }

        void EncryptAsyncCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            backgroundWorker.DoWork -= EncyrptAsyncBackgroundWorker;
            backgroundWorker.RunWorkerCompleted -= EncryptAsyncCompleted;
            base.AES_Completed();
            sw.Stop();

            if (e.Error != null){
                MessageBox.Show("Error: " + e.Error.Message 
                    + "\n\n Details: " + e.Error.StackTrace);
            }
            else {
                MessageBox.Show("Succes!\nFile is encprypted.\nEncrytpion time = "
                + sw.ElapsedMilliseconds + "ms.");
            }
        }

        void EncyrptAsyncBackgroundWorker(object sender, DoWorkEventArgs e)
        {
            DataForEnc data = (DataForEnc)e.Argument;

            byte[] saltBytes = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };
            string cryptFile = data.OutputFile;

            FileStream fsCrypt = new FileStream(cryptFile, FileMode.Append);

            RijndaelManaged AES = new RijndaelManaged();

            AES.KeySize = data.KeySize;
            AES.BlockSize = data.BlockSize;

            var key = new Rfc2898DeriveBytes(data.AES_KeyBytes, saltBytes, 1000);
            AES.Key = key.GetBytes(AES.KeySize / 8);
            AES.IV = key.GetBytes(AES.BlockSize / 8);
            AES.Padding = PaddingMode.Zeros;

            AES.Mode = data.CipherMode != CipherMode.OFB ? data.CipherMode : CipherMode.CBC;

            CryptoStream cs = new CryptoStream(fsCrypt,
                 AES.CreateEncryptor(),
                CryptoStreamMode.Write);

            FileStream fsIn = new FileStream(data.InputFile, FileMode.Open);

            int encryptedData;
            long lenStream = fsIn.Length;

            sw.Start();

            int prevVal = 0;
            long i = 0;
            for (; (encryptedData = fsIn.ReadByte()) != -1; i++)
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
        }

        private Stopwatch sw = new Stopwatch();
    }
}
