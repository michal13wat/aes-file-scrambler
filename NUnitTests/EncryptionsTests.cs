using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Security.Cryptography;
using System.IO;

namespace NUnitTests
{

    [TestFixture]
    public class EncryptionsTests
    {
        [Test, Order(1)]
        public void testThatPrevFilesDoesNotExists()
        {
            File.Delete(encryptedFile);
            File.Delete(decryptedFile);

            Assert.IsFalse(File.Exists(encryptedFile));
            Assert.IsFalse(File.Exists(decryptedFile));
        }

        //[Test, Order(2)]
        //public void testEncryptionFile()
        //{

        //    AESFileScrambler.Encryption.AES_Encrypt(inputFile,
        //        encryptedFile,
        //        mySHA256.ComputeHash(Encoding.ASCII.GetBytes("asdf")),
        //        cipherMode);

        //    Assert.IsTrue(File.Exists(encryptedFile));
        //}

        //[Test, Order(3)]
        //public void testDecryptionFile()
        //{

        //    AESFileScrambler.Encryption.AES_Decrypt(encryptedFile,
        //        decryptedFile,
        //        mySHA256.ComputeHash(Encoding.ASCII.GetBytes("asdf")),
        //        cipherMode);

        //    Assert.IsTrue(File.Exists(encryptedFile));
        //}

        [Test, Order(4)]
        public void compareInAndOutFiles() {
            //Assert.IsFalse(File.ReadAllBytes(inputFile).SequenceEqual(File.ReadAllBytes("E:\\mojfolder\\Studia\\semestr_6\\BSK\\Projekty\\BSK_projekt_1.pdf")));
            Assert.IsFalse(ReadBytes(inputFile, encryptedFile));
        }

        private string GetChecksum(string file)
        {
            using (FileStream stream = new FileStream(file, FileMode.Open))
            {
                //FileStream filestream;

                //filestream = new FileStream(file, FileMode.Open);
                stream.Position = 0;
                byte[] hashValue = mySHA256.ComputeHash(stream);
                string outHash = BitConverter.ToString(hashValue).Replace("-", String.Empty);
                TestContext.Out.WriteLine("Output Hash = " + outHash);
                stream.Close();
                return outHash;
            }
        }

        private bool ReadBytes(string file1, string file2)
        {
            byte[] bytes1 = File.ReadAllBytes(file1);
            byte[] bytes2 = File.ReadAllBytes(file2);

            if (bytes1.Length != bytes2.Length)
                return false;

            for(int i = 0; i < bytes1.Length; i++)
            {
                if (bytes1[i] != bytes2[i])
                    return false;
            }

            return true;
        }

        private const string inputFile 
            = "E:\\mojfolder\\Studia\\semestr_6\\BSK\\Projekty\\projekt1\\AESFileScrambler\\NUnitTests\\bin\\Debug\\in\\myFile.zip";
        private const string encryptedFile
            = "E:\\mojfolder\\Studia\\semestr_6\\BSK\\Projekty\\projekt1\\AESFileScrambler\\NUnitTests\\bin\\Debug\\encryptedFile";
        private const string decryptedFile
            = "E:\\mojfolder\\Studia\\semestr_6\\BSK\\Projekty\\projekt1\\AESFileScrambler\\NUnitTests\\bin\\Debug\\out\\myFile.zip";
        private CipherMode cipherMode = CipherMode.CBC;
        private SHA256 mySHA256 = SHA256.Create();
    }
}
