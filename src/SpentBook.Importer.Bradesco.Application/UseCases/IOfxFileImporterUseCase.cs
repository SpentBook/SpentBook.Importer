using System.Threading.Tasks;

namespace SpentBook.Importer.Bradesco.Application.UseCases
{
    public interface IOfxFileImporterUseCase
    {
        Task Run();
    }
}