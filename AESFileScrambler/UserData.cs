using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace AESFileScrambler
{
    public class UserData
    {
        public string Name { get; set; }
        public byte [] Passwd { get; set; }
        public byte [] PasswdHash { get {
                return mySHA256.ComputeHash(Passwd);
            }
        }
        public RSAParameters PubKey { get; set; }
        public RSAParameters PrivKey { get; set; }
        public byte [] PlainSesKey { get; set; }
        public byte [] EncSesKey { get; set; }

        private SHA256 mySHA256 = SHA256.Create();
    }
}
