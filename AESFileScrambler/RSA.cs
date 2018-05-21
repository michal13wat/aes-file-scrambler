using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace AESFileScrambler
{
    class RSA
    {

        public string EncryptSessionKey(string sessionKey, string hashPassword, string user) {
            RSACryptoServiceProvider csp = new RSACryptoServiceProvider(2048);

            //how to get the private key
            PrivateKey = csp.ExportParameters(true);

            //and the public key ...
            PublicKey = csp.ExportParameters(false);

            csp = new RSACryptoServiceProvider();
            csp.ImportParameters(PublicKey);

            //for encryption, always handle bytes...
            var bytesPlainTextData = System.Text.Encoding.Unicode.GetBytes(sessionKey);

            //apply pkcs#1.5 padding and encrypt our data 
            var bytesCypherText = csp.Encrypt(bytesPlainTextData, false);

            //we might want a string representation of our cypher text... base64 will do
            return Convert.ToBase64String(bytesCypherText);
        }

        public string DecryptSessionKey(string encryptedSessionyKey, string hashPassword, string user) {
            //first, get our bytes back from the base64 string ...
            var bytesCypherText = Convert.FromBase64String(encryptedSessionyKey);

            csp = new RSACryptoServiceProvider();
            csp.ImportParameters(PrivateKey);

            //decrypt and strip pkcs#1.5 padding
            var bytesPlainTextData = csp.Decrypt(bytesCypherText, false);

            //get our original plainText back...
            return System.Text.Encoding.Unicode.GetString(bytesPlainTextData);
        }

        private string convertKeyToString(string key) {
            var sw = new System.IO.StringWriter();
            //we need a serializer
            var xs = new System.Xml.Serialization.XmlSerializer(typeof(RSAParameters));
            //serialize the key into the stream
            xs.Serialize(sw, key);
            //get the string from the stream
            return sw.ToString();
        }

        private RSAParameters convertKeyToRSAParameters(string key) {
            var sr = new System.IO.StringReader(key);
            //we need a deserializer
            var xs = new System.Xml.Serialization.XmlSerializer(typeof(RSAParameters));
            //get the object back from the stream
            return (RSAParameters)xs.Deserialize(sr);
        }
        
        public RSAParameters PrivateKey { get; internal set; }
        public RSAParameters PublicKey { get; internal set; }

        private RSACryptoServiceProvider csp;
    }
}
