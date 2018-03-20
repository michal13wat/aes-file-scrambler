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
    public class Class1
    {
        [Test, Order(1)]
        public void testThatPrevFilesDoesNotExists() {
            File.Delete(encryptedFile);
            File.Delete(decryptedFile);

            Assert.IsFalse(File.Exists(encryptedFile));
            Assert.IsFalse(File.Exists(decryptedFile));
        }

        [Test, Order(2)]
        public void testEncryptionFile() {
            
            AESFileScrambler.Encryption.AES_Encrypt(inputFile,
                encryptedFile, 
                mySHA256.ComputeHash(Encoding.ASCII.GetBytes("asdf")),
                cipherMode);

            Assert.IsTrue(File.Exists(encryptedFile));
        }

        [Test, Order(3)]
        public void testDecryptionFile()
        {

            AESFileScrambler.Encryption.AES_Decrypt(encryptedFile,
                decryptedFile,
                mySHA256.ComputeHash(Encoding.ASCII.GetBytes("asdf")),
                cipherMode);

            Assert.IsTrue(File.Exists(encryptedFile));
        }

        private const string inputFile 
            = "E:\\mojfolder\\Studia\\semestr_6\\BSK\\Projekty\\projekt1\\AESFileScrambler\\NUnitTests\\bin\\Debug\\sourceFile.zip";
        private const string encryptedFile
            = "E:\\mojfolder\\Studia\\semestr_6\\BSK\\Projekty\\projekt1\\AESFileScrambler\\NUnitTests\\bin\\Debug\\encryptedFile";
        private const string decryptedFile
            = "E:\\mojfolder\\Studia\\semestr_6\\BSK\\Projekty\\projekt1\\AESFileScrambler\\NUnitTests\\bin\\Debug\\decryptedFile.zip";
        private CipherMode cipherMode = CipherMode.CBC;
        private SHA256 mySHA256 = SHA256.Create();
    }
}
