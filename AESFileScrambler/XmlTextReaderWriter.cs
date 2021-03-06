﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;

namespace AESFileScrambler
{
    class XmlTextReaderWriter
    {
        public XmlTextReaderWriter(CommonDataEncDec data) {
            settings.Indent = true;
            settings.IndentChars = "\t";
            this.data = data;
        }

        public void WriteToXml() {
            XmlWriter writer = XmlWriter.Create(data.OutputFile, settings);

            writer.WriteStartElement("x", "root", "a");

            writer.WriteStartElement(XmlConstants.ALGORITHM);
            writer.WriteAttributeString("xmlns", "x", null, "AES");
            writer.WriteEndElement();

            writer.WriteStartElement(XmlConstants.KEY_SIZE);
            writer.WriteAttributeString("xmlns", "x", null, data.KeySize.ToString());
            writer.WriteEndElement();

            writer.WriteStartElement(XmlConstants.BLOCK_SIZE);
            writer.WriteAttributeString("xmlns", "x", null, data.BlockSize.ToString());
            writer.WriteEndElement();

            writer.WriteStartElement(XmlConstants.CIPHER_MODE);
            writer.WriteAttributeString("xmlns", "x", null, data.StringCipherMode);
            writer.WriteEndElement();

            writer.WriteStartElement(XmlConstants.FILE_EXTENSION);
            writer.WriteAttributeString("xmlns", "x", null, data.FileExtension);
            writer.WriteEndElement();

            writer.WriteStartElement(XmlConstants.APPROVED_USERS);

            foreach (KeyValuePair<string, UserData> up in data.UsersCollection) {
                writer.WriteStartElement(XmlConstants.USER);
                writer.WriteAttributeString("xmlns", "x", null, up.Key);
                writer.WriteEndElement();

                writer.WriteStartElement(XmlConstants.SESSION_KEY);
                writer.WriteAttributeString("xmlns", "x", null, Convert.ToBase64String(up.Value.EncSesKey));
                writer.WriteEndElement();
            }

            writer.WriteEndElement();

            writer.WriteEndElement();
            writer.Close();

            using (FileStream fs = new FileStream(data.OutputFile, FileMode.Append, FileAccess.Write))
            using (StreamWriter sw = new StreamWriter(fs))
            {
                sw.WriteLine("\n" + delimiter);
            }
        }

        public DataForDec ReadXml() {
            DataForDec dataForDec = new DataForDec();

            string xmlString = "";
            try {
                using (FileStream fs = new FileStream(data.InputFile, FileMode.Open, FileAccess.Read))
                using (StreamReader sw = new StreamReader(fs))
                {
                    string line = "";
                    while (true)
                    {
                        line = sw.ReadLine();

                        if (line.Equals(delimiter)){
                            break;
                        }
                        xmlString += line;
                    }
                }

            }
            catch {
                MessageBox.Show("Can not open file input file to decrypted!");
                return dataForDec;
            }

            using (XmlReader reader
                = XmlReader.Create(new StringReader(xmlString)))
            {

                reader.ReadToFollowing(XmlConstants.KEY_SIZE);
                reader.MoveToFirstAttribute();
                dataForDec.KeySize = int.Parse(reader.Value);

                reader.ReadToFollowing(XmlConstants.BLOCK_SIZE);
                reader.MoveToFirstAttribute();
                dataForDec.BlockSize = int.Parse(reader.Value);

                reader.ReadToFollowing(XmlConstants.CIPHER_MODE);
                reader.MoveToFirstAttribute();
                dataForDec.StringCipherMode = reader.Value;

                reader.ReadToFollowing(XmlConstants.FILE_EXTENSION);
                reader.MoveToFirstAttribute();
                dataForDec.FileExtension = reader.Value;

                string user = "...";
                string sesionKey = "...";
                while (true)
                {
                    try
                    {
                        reader.ReadToFollowing(XmlConstants.USER);
                        reader.MoveToFirstAttribute();
                        user = reader.Value;

                        reader.ReadToFollowing(XmlConstants.SESSION_KEY);
                        reader.MoveToFirstAttribute();
                        sesionKey = reader.Value;

                        if ("".Equals(user) || "".Equals(sesionKey)) break;

                        dataForDec.UsersCollection.Add(user, new UserData() { EncSesKey = Convert.FromBase64String(sesionKey) });
                    }
                    catch{
                        break;
                    }
                }
            }

            return dataForDec;
        }

        private XmlWriterSettings settings = new XmlWriterSettings();
        private CommonDataEncDec data;
        private const string delimiter = "=====================================================";
    }
}
