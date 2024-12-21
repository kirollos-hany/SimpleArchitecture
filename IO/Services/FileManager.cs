using SimpleArchitecture.IO.Interfaces;
using SimpleArchitecture.Web.Interfaces;

namespace SimpleArchitecture.IO.Services;

internal sealed class FileManager : IFileManager
{
    private readonly IContentTypeProvider _contentTypeProvider;

    private readonly IBaseUrlProvider _baseUrlProvider;

    private readonly IWebHostEnvironment _webHostEnvironment;

    public FileManager(IContentTypeProvider contentTypeProvider, IBaseUrlProvider baseUrlProvider,
        IWebHostEnvironment webHostEnvironment)
    {
        _contentTypeProvider = contentTypeProvider;
        _baseUrlProvider = baseUrlProvider;
        _webHostEnvironment = webHostEnvironment;
    }

    public async Task<string> SaveAsync(IFormFile file, string fileDirectory = "")
    {
        var rootDirectory = _webHostEnvironment.WebRootPath;

        var fileExtension = new FileInfo(file.Name).Extension;

        var fileId = Guid.NewGuid() + fileExtension;

        var completePath = Path.Combine(rootDirectory, fileDirectory, fileId);

        await using var fileStream = new FileStream(completePath, FileMode.CreateNew);

        await using var memoryStream = new MemoryStream();

        await file.OpenReadStream().CopyToAsync(memoryStream);

        await fileStream.WriteAsync(memoryStream.ToArray());

        return Path.Combine(fileDirectory, fileId);
    }

    public async Task<string> UpdateAsync(string oldFileId, IFormFile newFile, string fileDirectory = "")
    {
        var rootDirectory = _webHostEnvironment.WebRootPath;

        var oldFilePath = Path.Combine(rootDirectory, oldFileId);

        File.Delete(oldFilePath);

        var newFileExtension = new FileInfo(newFile.Name).Extension;

        var newFileId = Guid.NewGuid() + newFileExtension;

        var newCompletePath = Path.Combine(rootDirectory, fileDirectory, newFileId);

        await using var fileStream = new FileStream(newCompletePath, FileMode.CreateNew);

        await using var memoryStream = new MemoryStream();

        await newFile.OpenReadStream().CopyToAsync(memoryStream);

        await fileStream.WriteAsync(memoryStream.ToArray());

        return Path.Combine(fileDirectory, newFileId);
    }

    public Task DeleteAsync(string fileId)
    {
        var rootDirectory = _webHostEnvironment.WebRootPath;

        var filePath = Path.Combine(rootDirectory, fileId);

        File.Delete(filePath);

        return Task.CompletedTask;
    }

    public Task<string> GetUrlAsync(string fileId)
    {
        return Task.FromResult(_baseUrlProvider.GetBaseUrl() + fileId);
    }

    public string GetContentType(string fileName)
    {
        if (!_contentTypeProvider.TryGetContentType(fileName, out var contentType))
        {
            contentType = "application/octet-stream";
        }

        return contentType;
    }
}