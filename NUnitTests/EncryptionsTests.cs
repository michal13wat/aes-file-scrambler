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
        //byte by byte


        [Test, Order(4)]
        public void compareInAndOutFiles() {
            Assert.IsTrue(CompareByBytes(new FileInfo(inputFile), new FileInfo(decryptedFile)));
            Assert.IsTrue(CompareByHash_MD5( new FileInfo(inputFile), new FileInfo(decryptedFile)));
            Assert.IsTrue(CompareByHash_SHA256(new FileInfo(inputFile), new FileInfo(decryptedFile)));
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
