using Microsoft.AspNetCore.Http;

namespace Core.Application.Services
{
    public interface IFileService
    {
        Task<List<string>> WriteFile(List<IFormFile> files);
    }
}
