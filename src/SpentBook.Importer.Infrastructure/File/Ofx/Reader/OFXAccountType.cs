using System.ComponentModel;

namespace SpentBook.Importer.Infrastructure.File.Ofx.Reader
{
    public enum OFXAccountType
    {
        [Description("Bank Account")]
        BANK,
        [Description("Credit Card")]
        CC,
        [Description("Accounts Payable")]
        AP,
        [Description("Accounts Recievable")]
        AR,
        NA,
    }
}