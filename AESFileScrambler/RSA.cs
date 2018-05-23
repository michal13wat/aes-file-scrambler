using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Xml;

namespace AESFileScrambler
{
    class RSA
    {

        public UserData EncryptSessionKey(UserData userData) {
            RSACryptoServiceProvider csp = new RSACryptoServiceProvider(2048);

            //how to get the private key
            userData.PrivKey = csp.ExportParameters(true);

            //and the public key ...
            userData.PubKey = csp.ExportParameters(false);

            csp = new RSACryptoServiceProvider();
            csp.ImportParameters(userData.PubKey);

            //apply pkcs#1.5 padding and encrypt our data 
            userData.EncSesKey = csp.Encrypt(userData.PlainSesKey, false);

            //we might want a string representation of our cypher text... base64 will do
            return userData;
        }

        public UserData DecryptSessionKey(UserData userData) {

            RSACryptoServiceProvider csp = new RSACryptoServiceProvider();

            try {
                csp.ImportParameters(userData.PrivKey);

                //decrypt and strip pkcs#1.5 padding
                userData.PlainSesKey = csp.Decrypt(userData.EncSesKey, false);
            }
            catch {
                MessageBox.Show("RSA error: can not decrypt session key!");
                return userData;
            }

            //get our original plainText back...
            return userData;
        }

        public void writeRSAParametersToFile(RSAParameters rsaParameters, string fileName) {

            string stringKey;
            {
                //we need some buffer
                var sw = new System.IO.StringWriter();
                //we need a serializer
                var xs = new System.Xml.Serialization.XmlSerializer(typeof(RSAParameters));
                //serialize the key into the stream
                xs.Serialize(sw, rsaParameters);
                //get the string from the stream
                stringKey = sw.ToString();
            }

            using (FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write))
            using (StreamWriter sw = new StreamWriter(fs))
            {
                sw.WriteLine(stringKey);
            }
        }

        public RSAParameters readRSAParametersFromFile(string fileName)
        {
            RSAParameters readParameters = new RSAParameters();

            string stringKey = "";
            try
            {
                using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read))
                using (StreamReader sw = new StreamReader(fs))
                {
                    stringKey = sw.ReadToEnd();
                }

            }
            catch
            {
                MessageBox.Show("Can not open file with private key!");
                return readParameters;
            }

            //get a stream from the string
            var sr = new System.IO.StringReader(stringKey);
            //we need a deserializer
            var xs = new System.Xml.Serialization.XmlSerializer(typeof(RSAParameters));
            //get the object back from the stream
            readParameters = (RSAParameters)xs.Deserialize(sr);

            return readParameters;
        }
    }                        
}
