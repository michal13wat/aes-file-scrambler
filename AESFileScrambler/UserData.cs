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
        //public static List<UserPasswd> Data = new List<UserPasswd>();
        public string Name { get; set; }
        public byte [] Passwd { get; set; }
        public byte [] PasswdHash { get {
                return mySHA256.ComputeHash(Passwd);
            }
        }
        public string PubKey { get; set; }
        public string PrivKey { get; set; }
        public string PlainSesKey { get; set; }
        public byte [] EncSesKey { get; set; }

        private SHA256 mySHA256 = SHA256.Create();
    }
}
