namespace SimpleArchitecture.IO.Interfaces;

public interface IFileManager
{
    Task<string> SaveAsync(IFormFile file, string fileDirectory = "");
    Task<string> UpdateAsync(string oldFileId, IFormFile newFile, string fileDirectory = "");
    Task DeleteAsync(string fileId);
    Task<string> GetUrlAsync(string fileId);
    string GetContentType(string fileName);
}