using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace AESFileScrambler
{
    public class AES_Configuration
    {
        public static string encInFile = "E:\\mojfolder\\Studia\\semestr_6\\BSK\\Projekty\\projekt1\\inExFiles\\1.jpg";
        public static string encOutFile = "E:\\mojfolder\\Studia\\semestr_6\\BSK\\Projekty\\projekt1\\encryptedFile";
        public static string decInFile = "E:\\mojfolder\\Studia\\semestr_6\\BSK\\Projekty\\projekt1\\encryptedFile";
        public static string decOutFile = "E:\\mojfolder\\Studia\\semestr_6\\BSK\\Projekty\\projekt1\\outExFiles\\decryptedFile";

        public static CipherMode cipherMode = CipherMode.CBC;
    }
}
