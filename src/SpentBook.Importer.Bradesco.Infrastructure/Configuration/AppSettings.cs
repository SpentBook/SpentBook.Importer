using System;
using System.Collections.Generic;
using System.Text;

namespace SpentBook.Importer.Bradesco.Infrastructure.Configuration
{
    public class AppSettings
    {
        public string SourceFolder { get; set; }
        public string ErrorFolder { get; set; }
        public string ProcessedFolder { get; set; }
        public string WorkingFolder { get; set; }
        public string DataBaseName { get; set; }
        public string MigrationAssemblyMySql { get; set; }
        public string MigrationAssemblySqlServer { get; set; }
    }
}
