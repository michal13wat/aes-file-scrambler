using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AESFileScrambler
{
    public class PrimeNumberGenerator
    {
        static public Org.BouncyCastle.Math.BigInteger genpr2(int bits, long initialRand)
        {
#pragma warning disable CS0618 // Type or member is obsolete
            Org.BouncyCastle.Security.SecureRandom ran = new Org.BouncyCastle.Security.SecureRandom(
                new Org.BouncyCastle.Math.BigInteger(initialRand.ToString()).ToByteArray());
#pragma warning restore CS0618 // Type or member is obsolete

            Org.BouncyCastle.Math.BigInteger c = new Org.BouncyCastle.Math.BigInteger(bits, ran);

            for (;;)
            {
                if (c.IsProbablePrime(1) == true) break;

                c = c.Subtract(new Org.BouncyCastle.Math.BigInteger("1"));
            }
            return (c);
        }
    }
}
