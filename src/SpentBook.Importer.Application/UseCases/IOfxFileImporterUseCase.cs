using System.Threading.Tasks;

namespace SpentBook.Importer.Application.UseCases
{
    public interface IOfxFileImporterUseCase
    {
        Task Run();
    }
}