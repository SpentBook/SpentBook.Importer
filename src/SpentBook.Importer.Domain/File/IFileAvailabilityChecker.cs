namespace SpentBook.Importer.Domain.File
{
    public interface IFileAvailabilityChecker
    {
        void CheckForAvailableFilesAndMoveToWorking(string extension);
    }
}
