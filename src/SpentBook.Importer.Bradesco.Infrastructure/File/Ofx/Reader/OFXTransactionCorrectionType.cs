using System.ComponentModel;

namespace SpentBook.Importer.Bradesco.Infrastructure.File.Ofx.Reader
{
    public enum OFXTransactionCorrectionType
    {
        [Description("No correction needed")]
        NA,
        [Description("Replace this transaction with one referenced by CORRECTFITID")]
        REPLACE,
        [Description("Delete transaction")]
        DELETE,
    }
}