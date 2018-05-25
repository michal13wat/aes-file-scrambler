using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AESFileScrambler
{
    public class AES_AsyncDecryptionFile : AES_AsyncCommon
    {
        public AES_AsyncDecryptionFile()
        {
            backgroundWorker.DoWork += DecyrptAsyncBackgroundWorker;
            backgroundWorker.RunWorkerCompleted += DecryptAsyncCompleted;
        }

        void DecryptAsyncCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            backgroundWorker.DoWork -= DecyrptAsyncBackgroundWorker;
            backgroundWorker.RunWorkerCompleted -= DecryptAsyncCompleted;
            base.AES_Completed();

            sw.Stop();

            if (e.Error != null){
                MessageBox.Show("Error: " + e.Error.Message);
            }
            else {
                MessageBox.Show("Succes!\nFile is decrypted.\nDecrytpion time = "
                    + sw.ElapsedMilliseconds + "ms.");
            }
        }

        void DecyrptAsyncBackgroundWorker(object sender, DoWorkEventArgs e)
        {
            DataForDec data = (DataForDec)e.Argument;

            byte[] saltBytes = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };

            FileStream fsCrypt = new FileStream(data.InputFile, FileMode.Open);
            Int32 position = 0;
            using (BinaryReader reader = new BinaryReader(fsCrypt))
            {
                byte prevByte = 0;
                byte readByte;
                int delimeterSignsCounter = 0;

                while (true)
                {
                    readByte = reader.ReadByte();
                    position++;
                    if (readByte == 0x3d && prevByte == 0x3d)
                    {
                        delimeterSignsCounter++;
                        if(delimeterSignsCounter >= 52)
                        {
                            fsCrypt.Seek(2, SeekOrigin.Current);
                            break;
                        }
                    }
                    else delimeterSignsCounter = 0;

                    prevByte = readByte;
                }

                RijndaelManaged AES = new RijndaelManaged();

                AES.KeySize = data.KeySize;
                AES.BlockSize = data.BlockSize;

                if (data.AES_KeyBytes == null) throw new Exception("Key bytes are NULL!!!");

                var key = new Rfc2898DeriveBytes(data.AES_KeyBytes, saltBytes, 1000);
                AES.Key = key.GetBytes(AES.KeySize / 8);
                AES.IV = key.GetBytes(AES.BlockSize / 8);
                AES.Padding = PaddingMode.Zeros;

                AES.Mode = data.CipherMode != CipherMode.OFB ? data.CipherMode : CipherMode.CBC;

                CryptoStream cs = new CryptoStream(fsCrypt,
                    AES.CreateDecryptor(),
                    CryptoStreamMode.Read);

                FileStream fsOut = new FileStream(data.OutputFile, FileMode.Create);

                int encryptedData;
                long lenStream = fsCrypt.Length;

                int prevVal = 0;

                sw.Start();

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
        private Stopwatch sw = new Stopwatch();
    }
}
