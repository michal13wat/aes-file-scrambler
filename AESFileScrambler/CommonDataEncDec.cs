using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace AESFileScrambler
{
    abstract class CommonDataEncDec
    {
        public string InputFile { get; set; }
        public string OutputFile { get; set; }
        public byte[] AES_KeyBytes { get; set; }
        public int KeySize { get; set; }
        public int BlockSize { get; set; }
        public CipherMode CipherMode { get; set; }
        public string StringCipherMode { get; set; }
        public Dictionary<string, byte[]> RSA_UsersKeys = new Dictionary<string, byte[]>();
    }
}
