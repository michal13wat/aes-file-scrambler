using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Security.Cryptography;
using System.IO;
using AESFileScrambler;
using System.Threading;

namespace NUnitTests
{

    [TestFixture]
    public class EncryptionsTests
    {
        [Test, Order(1)]
        public void testThatPrevFilesDoesNotExists()
        {
            File.Delete(AES_Configuration.encOutDirectory);
            File.Delete(AES_Configuration.decOutDirectory);

            Assert.IsFalse(File.Exists(AES_Configuration.encOutDirectory));
            Assert.IsFalse(File.Exists(AES_Configuration.decOutDirectory));
        }

        [Test, Order(2)]
        public void testEncryptionFile()
        {
            //AES_Configuration.secretPrimeNumber = PrimeNumberGenerator.genpr2(128);

            DataForEnc data = new DataForEnc();
            data.InputFile = AES_Configuration.encInFile;
            data.OutputFile = AES_Configuration.encOutDirectory;
            //data.AES_KeyBytes = AES_Configuration.secretPrimeNumber.ToByteArray();  // mySHA256.ComputeHash(secretPrimeNumber);
            data.CipherMode = AES_Configuration.cipherMode;
            data.KeySize = 128;
            data.BlockSize = 128;
            data.UsersCollection.Add("John", new UserData()
            { Name = "John", Passwd = mySHA256.ComputeHash(Encoding.ASCII.GetBytes("asdf")) }); 

            AES_AsyncEncryptionFile asyncEnc = new AES_AsyncEncryptionFile();
            //asyncEnc.backgroundWorker.ProgressChanged += updateEncProgressBar;
            asyncEnc.backgroundWorker.RunWorkerAsync(data);

            waitForEndBackgroundWorkder(asyncEnc);

            Assert.IsTrue(File.Exists(AES_Configuration.encOutDirectory));
        }

        [Test, Order(3)]
        public void testDecryptionFile()
        {
            CommonDataEncDec dataForDec = new DataForDec();
            dataForDec.CipherMode = AES_Configuration.cipherMode;
            dataForDec.InputFile = AES_Configuration.decInFile;
            dataForDec.OutputFile = AES_Configuration.decOutDirectory;
            //dataForDec.AES_KeyBytes = AES_Configuration.secretPrimeNumber.ToByteArray();  //mySHA256.ComputeHash(passwdHash);
            dataForDec.KeySize = 128;
            dataForDec.BlockSize = 128;

            AES_AsyncDecryptionFile asyncDec = new AES_AsyncDecryptionFile();
            //asyncDec.backgroundWorker.ProgressChanged += updateDecProgressBar;
            asyncDec.backgroundWorker.RunWorkerAsync(dataForDec);

            waitForEndBackgroundWorkder(asyncDec);

            Assert.IsTrue(File.Exists(AES_Configuration.decOutDirectory));
        }
        //byte by byte


        [Test, Order(4)]
        public void compareInAndOutFilesByBytes() {
            Assert.IsTrue(CompareByBytes(new FileInfo(AES_Configuration.encInFile), new FileInfo(AES_Configuration.decOutDirectory)));
        }

        [Test, Order(5)]
        public void compareInAndOutFilesByMD5()
        {
            Assert.IsTrue(CompareByHash_MD5(new FileInfo(AES_Configuration.encInFile), new FileInfo(AES_Configuration.decOutDirectory)));
        }

        [Test, Order(6)]
        public void compareInAndOutFilesBySHA256()
        {
            Assert.IsTrue(CompareByHash_SHA256(new FileInfo(AES_Configuration.encInFile), new FileInfo(AES_Configuration.decOutDirectory)));
        }


        private void waitForEndBackgroundWorkder(AES_AsyncCommon asyncEnc) {
            while (asyncEnc.backgroundWorker.IsBusy) Thread.Sleep(200);
        }

        private bool CompareByBytes(FileInfo file1, FileInfo file2)
        {
            if (file1.Length != file2.Length)
                return false;

            using (FileStream fs1 = file1.OpenRead())
            using (FileStream fs2 = file2.OpenRead())
            {
                for (int i = 0; i < file1.Length; i++)
                {
                    if (fs1.ReadByte() != fs2.ReadByte())
                        return false;
                }
            }

            return true;
        }

        private bool CompareByHash_MD5(FileInfo file1, FileInfo file2)
        {
            byte[] file1Hash = MD5.Create().ComputeHash(file1.OpenRead());
            byte[] file2Hash = MD5.Create().ComputeHash(file2.OpenRead());

            for (int i = 0; i < file1Hash.Length; i++)
            {
                if (file1Hash[i] != file2Hash[i])
                    return false;
            }

            return true;
        }

        private bool CompareByHash_SHA256(FileInfo file1, FileInfo file2)
        {
            byte[] file1Hash = SHA256.Create().ComputeHash(file1.OpenRead());
            byte[] file2Hash = SHA256.Create().ComputeHash(file2.OpenRead());

            for (int i = 0; i < file1Hash.Length; i++)
            {
                if (file1Hash[i] != file2Hash[i])
                    return false;
            }

            return true;
        }

        private SHA256 mySHA256 = SHA256.Create();
    }
}
