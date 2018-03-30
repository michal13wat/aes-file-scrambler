using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace AESFileScrambler
{
    public class AES_Configuration
    {
        public static string encInFile = "E:\\mojfolder\\Studia\\semestr_6\\BSK\\Projekty\\projekt1\\inExFiles\\3.zip";
        public static string encOutFile = "E:\\mojfolder\\Studia\\semestr_6\\BSK\\Projekty\\projekt1\\encryptedFile";
        public static string decInFile = "E:\\mojfolder\\Studia\\semestr_6\\BSK\\Projekty\\projekt1\\encryptedFile";
        public static string decOutFile = "E:\\mojfolder\\Studia\\semestr_6\\BSK\\Projekty\\projekt1\\outExFiles\\decryptedFile.zip";

        public static CipherMode cipherMode = CipherMode.CBC;
        public static Org.BouncyCastle.Math.BigInteger secretPrimeNumber = new Org.BouncyCastle.Math.BigInteger("0");

        public static Dictionary<string, byte[]> usersPasswords
            = new Dictionary<string, byte[]>();
    }
}
