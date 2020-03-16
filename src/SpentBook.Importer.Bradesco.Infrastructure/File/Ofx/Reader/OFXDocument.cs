using System;
using System.Collections.Generic;

namespace SpentBook.Importer.Bradesco.Infrastructure.File.Ofx.Reader
{
    public class OFXDocument
    {
        public DateTime StatementStart { get; set; }

        public DateTime StatementEnd { get; set; }

        public OFXAccountType AccType { get; set; }

        public string Currency { get; set; }

        public OFXSignOn SignOn { get; set; }

        public OFXAccount Account { get; set; }

        public OFXBalance Balance { get; set; }

        public List<OFXTransaction> Transactions { get; set; }
    }
}