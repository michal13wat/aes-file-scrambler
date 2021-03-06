﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace AESFileScrambler
{
    public abstract class CommonDataEncDec
    {
        public string InputFile {
            get {
                return inputFile;
            }
            set {
                inputFile = value;
                fileExtension = Path.GetExtension(value);
            }
        }
        public string OutputFile { get; set; }
        public byte[] AES_KeyBytes { get; set; }
        public int KeySize { get; set; }
        public int BlockSize { get; set; }

        public string FileExtension {
            get {
                return fileExtension;
            }
            set {
                fileExtension = value;
            }
        }

        public CipherMode CipherMode {
            get { return cipherMode; }
            set {
                cipherMode = value;
                stringCipherMode = mapStringEncModeToEnum(cipherMode);
            }
        }
        public string StringCipherMode {
            get { return stringCipherMode; }
            set {
                stringCipherMode = value;
                cipherMode = mapEncModeStringToEnum(stringCipherMode);
            }
        }

        public Dictionary<string, UserData> UsersCollection = new Dictionary<string, UserData>();

        private CipherMode mapEncModeStringToEnum(string modeName)
        {
            CipherMode encMode;

            string encModeString = modeName;
            switch (encModeString)
            {
                case "CBC": encMode = CipherMode.CBC; break;
                case "CFB": encMode = CipherMode.CFB; break;
                case "EBC": encMode = CipherMode.ECB; break;
                case "OFB": encMode = CipherMode.OFB; break;
                default: encMode = CipherMode.CBC; break;
            }
            return encMode;
        }

        private string mapStringEncModeToEnum(CipherMode cipherMode){
            switch (cipherMode) {
                case CipherMode.CBC:
                    return "CBC";
                case CipherMode.CFB:
                    return "CFB";
                case CipherMode.ECB:
                    return "EBC";
                case CipherMode.OFB:
                    return "OFB";
                default:
                    return "CBC";
            }
        }

        private CipherMode cipherMode;
        private string stringCipherMode;
        private string fileExtension;
        private string inputFile;
    }
}
