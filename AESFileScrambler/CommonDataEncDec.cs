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
        public byte[] PasswordBytes { get; set; }
        public CipherMode CipherMode { get; set; }
    }
}
