using System;
using System.Xml;

namespace SpentBook.Importer.Bradesco.Infrastructure.File.Ofx.Reader
{
    public class OFXAccount
    {
        public string AccountID { get; set; }
        public string AccountKey { get; set; }
        public OFXAccountType AccountType { get; set; }

        #region Bank Only

        private OFXBankAccountType _BankAccountType = OFXBankAccountType.NA;

        public string BankID { get; set; }

        public string BranchID { get; set; }


        public OFXBankAccountType BankAccountType
        {
            get
            {
                if (AccountType == OFXAccountType.BANK)
                    return _BankAccountType;

                return OFXBankAccountType.NA;
            }
            set
            {
                _BankAccountType = AccountType == OFXAccountType.BANK ? value : OFXBankAccountType.NA;
            }
        }

        #endregion

        public OFXAccount(XmlNode node, OFXAccountType type)
        {
            AccountType = type;

            AccountID = node.GetValue("//ACCTID");
            AccountKey = node.GetValue("//ACCTKEY");

            switch (AccountType)
            {
                case OFXAccountType.BANK:
                    InitializeBank(node);
                    break;
                case OFXAccountType.AP:
                    InitializeAP(node);
                    break;
                case OFXAccountType.AR:
                    InitializeAR(node);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Initializes information specific to bank
        /// </summary>
        private void InitializeBank(XmlNode node)
        {
            BankID = node.GetValue("//BANKID");
            BranchID = node.GetValue("//BRANCHID");

            //Get Bank Account Type from XML
            string bankAccountType = node.GetValue("//ACCTTYPE");

            //Check that it has been set
            if (string.IsNullOrEmpty(bankAccountType))
                throw new OFXParseException("Bank Account type unknown");

            //Set bank account enum
            _BankAccountType = bankAccountType.GetBankAccountType();
        }

        #region Account types not supported

        private void InitializeAP(XmlNode node)
        {
            throw new OFXParseException("AP Account type not supported");
        }

        private void InitializeAR(XmlNode node)
        {
            throw new OFXParseException("AR Account type not supported");
        }

        #endregion
    }
}