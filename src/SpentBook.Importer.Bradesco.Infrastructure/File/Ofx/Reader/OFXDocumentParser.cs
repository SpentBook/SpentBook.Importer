using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using OFXParser.Entities;
using Sgml;

namespace SpentBook.Importer.Bradesco.Infrastructure.File.Ofx.Reader
{
    public class OFXDocumentParser
    {
        public OFXDocument ParseOfxDocument(FileStream stream)
        {
            using (var reader = new StreamReader(stream))
            {
                return ParseOfxDocumentContent(reader.ReadToEnd());
            }
        }

        //private XmlDocument Parse2(string data)
        //{
        //    var xmlDocument = new XmlDocument();

        //    if (data.IndexOf("OFXHEADER") == -1)
        //    {
        //        throw new InvalidDataException();
        //    }

        //    var ofx = data.Remove(0, data.IndexOf("<"));

        //    var header = data;

        //    ofx = ConvertSGMLTOXML(ofx);

        //    xmlDocument.LoadXml(ofx);

        //    return xmlDocument;
        //}

        //private string ConvertSGMLTOXML(string sgml)
        //{
        //    using (var reader = new StringReader(sgml))
        //    {
        //        string line;

        //        var stringBuilder = new StringBuilder();

        //        while ((line = reader.ReadLine()) != null)
        //        {
        //            var tagEnd = line.IndexOf(">");

        //            if (tagEnd != line.Length - 1)
        //            {
        //                var tagStart = line.IndexOf("<");

        //                var tagName = line.Substring(tagStart + 1, (tagEnd - tagStart) - 1);

        //                if (line.IndexOf(string.Format("</{0}>", tagName)) > -1)
        //                {
        //                    stringBuilder.AppendLine(line);
        //                }
        //                else
        //                {
        //                    stringBuilder.AppendLine(String.Concat(line, string.Format("</{0}>", tagName)));
        //                }
        //            }
        //            else
        //            {
        //                stringBuilder.AppendLine(line);
        //            }
        //        }

        //        return stringBuilder.ToString();
        //    }
        //}

        public OFXDocument ParseOfxDocument(string path)
        {
            using (var reader = new StreamReader(path, true))
            {
                return ParseOfxDocumentContent(reader.ReadToEnd());
            }
        }

        public OFXDocument ParseOfxDocumentContent(string ofxContent)
        {
            //If OFX file in SGML format, convert to XML
            if (!IsXmlVersion(ofxContent))
            {
                ofxContent = SGMLToXML(ofxContent);
            }

            return ParseXml(ofxContent);
        }

        private OFXDocument ParseXml(string xml)
        {
            var ofx = new OFXDocument { AccType = GetAccountType(xml) };

            //Load into xml document
            var doc = new XmlDocument();
            doc.Load(new StringReader(xml));

            var currencyNode = doc.SelectSingleNode(GetXPath(ofx.AccType, OFXSection.CURRENCY));

            if (currencyNode != null)
            {
                ofx.Currency = currencyNode.FirstChild.Value;
            }
            else
            {
                throw new OFXParseException("Currency not found");
            }

            //Get sign on node from OFX file
            var signOnNode = doc.SelectSingleNode(Resources.SignOn);

            //If exists, populate signon obj, else throw parse error
            if (signOnNode != null)
            {
                ofx.SignOn = new OFXSignOn(signOnNode);
            }
            else
            {
                throw new OFXParseException("Sign On information not found");
            }

            //Get Account information for ofx doc
            var accountNode = doc.SelectSingleNode(GetXPath(ofx.AccType, OFXSection.ACCOUNTINFO));

            //If account info present, populate account object
            if (accountNode != null)
            {
                ofx.Account = new OFXAccount(accountNode, ofx.AccType);
            }
            else
            {
                //throw new OFXParseException("Account information not found");
            }

            //Get list of transactions
            ImportTransations(ofx, doc);

            //Get balance info from ofx doc
            var ledgerNode = doc.SelectSingleNode(GetXPath(ofx.AccType, OFXSection.BALANCE) + "/LEDGERBAL");
            var avaliableNode = doc.SelectSingleNode(GetXPath(ofx.AccType, OFXSection.BALANCE) + "/AVAILBAL");

            //If balance info present, populate balance object
            // ***** OFX files from my bank don't have the 'avaliableNode' node, so i manage a 'null' situation
            if (ledgerNode != null) // && avaliableNode != null
            {
                ofx.Balance = new OFXBalance(ledgerNode, avaliableNode);
            }
            else
            {
                throw new OFXParseException("Balance information not found");
            }

            return ofx;
        }


        /// <summary>
        /// Returns the correct xpath to specified section for given account type
        /// </summary>
        /// <param name="type">Account type</param>
        /// <param name="section">Section of OFX document, e.g. Transaction Section</param>
        /// <exception cref="OFXException">Thrown in account type not supported</exception>
        private string GetXPath(OFXAccountType type, OFXSection section)
        {
            string xpath, accountInfo;

            switch (type)
            {
                case OFXAccountType.BANK:
                    xpath = Resources.BankAccount;
                    accountInfo = "/BANKACCTFROM";
                    break;
                case OFXAccountType.CC:
                    xpath = Resources.CCAccount;
                    accountInfo = "/CCACCTFROM";
                    break;
                default:
                    throw new OFXException("Account Type not supported. Account type " + type);
            }

            switch (section)
            {
                case OFXSection.ACCOUNTINFO:
                    return xpath + accountInfo;
                case OFXSection.BALANCE:
                    return xpath;
                case OFXSection.TRANSACTIONS:
                    return xpath + "/BANKTRANLIST";
                case OFXSection.SIGNON:
                    return Resources.SignOn;
                case OFXSection.CURRENCY:
                    return xpath + "/CURDEF";
                default:
                    throw new OFXException("Unknown section found when retrieving XPath. Section " + section);
            }
        }

        /// <summary>
        /// Returns list of all transactions in OFX document
        /// </summary>
        /// <param name="doc">OFX document</param>
        /// <returns>List of transactions found in OFX document</returns>
        private void ImportTransations(OFXDocument ofxDocument, XmlDocument doc)
        {
            var xpath = GetXPath(ofxDocument.AccType, OFXSection.TRANSACTIONS);

            ofxDocument.StatementStart = doc.GetValue(xpath + "//DTSTART").ToDate();
            ofxDocument.StatementEnd = doc.GetValue(xpath + "//DTEND").ToDate();

            var transactionNodes = doc.SelectNodes(xpath + "//STMTTRN");

            ofxDocument.Transactions = new List<OFXTransaction>();

            foreach (XmlNode node in transactionNodes)
                ofxDocument.Transactions.Add(new OFXTransaction(node, ofxDocument.Currency));
        }


        /// <summary>
        /// Checks account type of supplied file
        /// </summaryof
        /// <param name="file">OFX file want to check</param>
        /// <returns>Account type for account supplied in ofx file</returns>
        private OFXAccountType GetAccountType(string file)
        {
            if (file.IndexOf("<CREDITCARDMSGSRSV1>") != -1)
                return OFXAccountType.CC;

            if (file.IndexOf("<BANKMSGSRSV1>") != -1)
                return OFXAccountType.BANK;

            throw new OFXException("Unsupported Account Type");
        }

        /// <summary>
        /// Check if OFX file is in SGML or XML format
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        private bool IsXmlVersion(string file)
        {
            return !Regex.IsMatch(file, "OFXHEADER:\\s*100");
        }

        /// <summary>
        /// Converts SGML to XML
        /// </summary>
        /// <param name="file">OFX File (SGML Format)</param>
        /// <returns>OFX File in XML format</returns>
        private string SGMLToXML(string file)
        {
            var reader = new SgmlReader();

            //Inititialize SGML reader
            reader.InputStream = new StringReader(ParseHeader(file));
            reader.DocType = "OFX";

            var sw = new StringWriter();
            var xml = new XmlTextWriter(sw);

            //write output of sgml reader to xml text writer
            while (!reader.EOF)
                xml.WriteNode(reader, true);

            //close xml text writer
            xml.Flush();
            xml.Close();

            var temp = sw.ToString().TrimStart().Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

            return string.Join("", temp);
        }

        /// <summary>
        /// Checks that the file is supported by checking the header. Removes the header.
        /// </summary>
        /// <param name="file">OFX file</param>
        /// <returns>File, without the header</returns>
        private string ParseHeader(string file)
        {
            //Select header of file and split into array
            //End of header worked out by finding first instance of '<'
            //Array split based of new line & carrige return
            var header = file.Substring(0, file.IndexOf('<'))
               .Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

            //Check that no errors in header
            //CheckHeader(header);

            //Remove header
            return file.Substring(file.IndexOf('<') - 1);
        }

        /// <summary>
        /// Checks that all the elements in the header are supported
        /// </summary>
        /// <param name="header">Header of OFX file in array</param>
        private void CheckHeader(string[] header)
        {
            if (!Regex.IsMatch(header[0], "OFXHEADER:\\s*100"))
                throw new OFXParseException("Incorrect header format");

            if (header[1] != "DATA:OFXSGML")
                throw new OFXParseException("Data type unsupported: " + header[1] + ". OFXSGML required");

            if (header[2] != "VERSION:102")
                throw new OFXParseException("OFX version unsupported. " + header[2]);

            if (header[3] != "SECURITY:NONE")
                throw new OFXParseException("OFX security unsupported");

            if (header[4] != "ENCODING:USASCII")
                throw new OFXParseException("ASCII Format unsupported:" + header[4]);

            if (header[5] != "CHARSET:1252")
                throw new OFXParseException("Charecter set unsupported:" + header[5]);

            if (header[6] != "COMPRESSION:NONE")
                throw new OFXParseException("Compression unsupported");

            if (header[7] != "OLDFILEUID:NONE")
                throw new OFXParseException("OLDFILEUID incorrect");
        }

        #region Nested type: OFXSection

        /// <summary>
        /// Section of OFX Document
        /// </summary>
        private enum OFXSection
        {
            SIGNON,
            ACCOUNTINFO,
            TRANSACTIONS,
            BALANCE,
            CURRENCY
        }

        #endregion
    }
}