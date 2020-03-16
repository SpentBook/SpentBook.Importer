namespace SpentBook.Importer.Bradesco.Domain.File
{
    public interface IFileAvailabilityChecker
    {
        void CheckForAvailableFilesAndMoveToWorking(string extension);
    }
}
