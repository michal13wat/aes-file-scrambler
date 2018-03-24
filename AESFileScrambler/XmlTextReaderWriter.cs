using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            writer.WriteAttributeString("xmlns", "x", null, "1");
            writer.WriteEndElement();

            writer.WriteStartElement(XmlConstants.BLOCK_SIZE);
            writer.WriteAttributeString("xmlns", "x", null, "2");
            writer.WriteEndElement();

            writer.WriteStartElement(XmlConstants.CIPHER_MODE);
            writer.WriteAttributeString("xmlns", "x", null, "3");
            writer.WriteEndElement();

            writer.WriteStartElement(XmlConstants.IV);
            writer.WriteAttributeString("xmlns", "x", null, "4");
            writer.WriteEndElement();

            writer.WriteStartElement(XmlConstants.APPROVED_USERS);

            writer.WriteStartElement(XmlConstants.USER);
            writer.WriteAttributeString("xmlns", "x", null, "asdf");
            writer.WriteEndElement();

            writer.WriteStartElement(XmlConstants.SESSION_KEY);
            writer.WriteAttributeString("xmlns", "x", null, "5");
            writer.WriteEndElement();

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
            using (FileStream fs = new FileStream(data.InputFile, FileMode.Open, FileAccess.Read))
            using (StreamReader sw = new StreamReader(fs))
            {
                string line = "";
                while (true)
                {
                    line = sw.ReadLine();

                    if (line.Equals(delimiter)) break;

                    xmlString += line;
                }
            }

            using (XmlReader reader
                = XmlReader.Create(new StringReader(xmlString)))
            {
                string genre;

                reader.ReadToFollowing("x:root");
                reader.MoveToFirstAttribute();
                genre = reader.Value;

                reader.ReadToFollowing(XmlConstants.ALGORITHM);
                reader.MoveToFirstAttribute();
                genre = reader.Value;

                reader.ReadToFollowing(XmlConstants.KEY_SIZE);
                reader.MoveToFirstAttribute();
                genre = reader.Value;

                reader.ReadToFollowing(XmlConstants.BLOCK_SIZE);
                reader.MoveToFirstAttribute();
                genre = reader.Value;

                reader.ReadToFollowing(XmlConstants.CIPHER_MODE);
                reader.MoveToFirstAttribute();
                genre = reader.Value;

                reader.ReadToFollowing(XmlConstants.IV);
                reader.MoveToFirstAttribute();
                genre = reader.Value;

                reader.ReadToFollowing(XmlConstants.USER);
                reader.MoveToFirstAttribute();
                genre = reader.Value;

                reader.ReadToFollowing(XmlConstants.USER);
                reader.MoveToNextAttribute();
                genre = reader.Value;

                reader.ReadToFollowing(XmlConstants.USER);
                reader.MoveToNextAttribute();
                genre = reader.Value;

                reader.ReadToFollowing(XmlConstants.USER);
                reader.MoveToNextAttribute();
                genre = reader.Value;

            }

            return dataForDec;
        }

        private XmlWriterSettings settings = new XmlWriterSettings();
        private CommonDataEncDec data;
        private const string delimiter = "=====================================================";
    }
}
