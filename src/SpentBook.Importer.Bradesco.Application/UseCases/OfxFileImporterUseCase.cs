using System;
using System.Threading.Tasks;

namespace SpentBook.Importer.Bradesco.Application.UseCases
{
    public class OfxFileImporterUseCase : IOfxFileImporterUseCase
    {
        public async Task Run()
        {
            Console.WriteLine("Executou: " + DateTime.Now.ToString());
        }
    }
}
