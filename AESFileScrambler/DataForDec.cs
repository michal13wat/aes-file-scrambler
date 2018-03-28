using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace AESFileScrambler
{
    class DataForDec : CommonDataEncDec
    {
        public long PositionReadingFile { get; set; }
    }
}
