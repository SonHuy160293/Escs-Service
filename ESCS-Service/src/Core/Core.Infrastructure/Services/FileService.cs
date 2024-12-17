using Core.Application.Common;
using Core.Application.Exceptions;
using Core.Application.Helpers;
using Core.Application.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Core.Infrastructure.Services
{
    public class FileService : IFileService
    {

        private readonly ILogger<FileService> _logger;


        public FileService(ILogger<FileService> logger)
        {
            _logger = logger;
        }

        public async Task<List<string>> WriteFile(List<IFormFile> files)
        {

            _logger.LogInformation("{Class} SAVING files to system", typeof(FileService).Name);

            try
            {
                var filesPath = new List<string>();

                foreach (var file in files)
                {
                    // Ensure the file name has an extension
                    var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(file.FileName);
                    var extension = Path.GetExtension(file.FileName);

                    // Generate a unique file ID (e.g., correlation ID)
                    var fileId = CorrelationIdProvider.GetCorrelationId() ?? Guid.NewGuid().ToString(); // Using GUID as fallback if CorrelationId is null

                    // Construct the new file name
                    var newFileName = $"{fileNameWithoutExtension}_{fileId}{extension}";


                    //get folder path to save file
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Upload\\Files");

                    //if folder do not exist then create
                    if (!Directory.Exists(filePath))
                    {
                        Directory.CreateDirectory(filePath);
                    }

                    var exactPath = Path.Combine(Directory.GetCurrentDirectory(), "Upload\\Files", newFileName);

                    using (var stream = new FileStream(exactPath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    filesPath.Add(exactPath);
                    //await _googleDriveService.UploadFileAsync(exactPath);
                }

                return filesPath;
            }
            catch (Exception ex)
            {
                var exceptionError = ExceptionError.Create(ex);

                _logger.LogError("{Class} SAVING files to system catch exception:{Exception}", typeof(FileService).Name, exceptionError);
                throw new BusinessException(ex.Message);
            }



        }
    }
}
