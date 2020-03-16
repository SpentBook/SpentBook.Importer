using System.ComponentModel;

namespace SpentBook.Importer.Bradesco.Infrastructure.File.Ofx.Reader
{
    public enum OFXBankAccountType
    {
        [Description("Checking Account")]
        CHECKING,
        [Description("Savings Account")]
        SAVINGS,
        [Description("Money Market Account")]
        MONEYMRKT,
        [Description("Line of Credit")]
        CREDITLINE,
        NA,
    }
}