
using GymManagment.BLL.Common;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagment.BLL.Services.Attachment
{
    public class AttachmentService : IAttachmentService
    {
        private readonly ILogger<AttachmentService> _logger;
        private readonly IWebHostEnvironment _webHost;
        private readonly int _maxFileSize;
        private readonly string[] _allowedExtention = { ".png",".jpeg" ,".jpg"};

        public AttachmentService(ILogger<AttachmentService> logger ,IWebHostEnvironment webHost)
        {
            _logger = logger;
           _webHost = webHost;
        }

        public Result Delete(string fileName, string folderName)
        {
           var filePath = Path.Combine(_webHost.ContentRootPath, folderName, fileName);
            if (!File.Exists(filePath))
            {
                _logger.LogError($"File Not Found : {filePath}");
                return Result.Fail("File Not Found");
            }
            try
            {
                File.Delete(filePath);
                return Result.Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed To Delete File {fileName}");
                return Result.Fail("Failed To Delete File");
            }
        }

        public (Stream stream, string ContentType)? GetFile(string fileName, string folderName)
        {
            if(string.IsNullOrWhiteSpace(fileName) || string.IsNullOrWhiteSpace(folderName))
            {
                _logger.LogError("Invalid File Name or Folder Name");
                return null;
            }

            var filePath = Path.Combine(_webHost.ContentRootPath, folderName, fileName);
            if (!File.Exists(filePath))
            {
                _logger.LogError($"File Not Found : {filePath}");
                return null;
            }

            try
            {
                var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                var extension = Path.GetExtension(fileName);
                var contentType = extension switch
                {
                    ".png" => "image/png",
                    ".jpg" => "image/jpeg",
                    ".jpeg" => "image/jpeg",
                    _ => "application/octet-stream"
                };
                return (fileStream, contentType);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed To Get File Path {fileName}");
                return null;
            }
        }

        public async Task<Result<string?>> UploadAsync(Stream filestream, string fileName, string folderName, CancellationToken ct = default)
        {
            if (filestream == null || !filestream.CanRead || filestream.Length == 0) {

                _logger.LogError($"File Rejected : File Too Large {filestream?.Length} Bytes");
                
                return Result<string?>.Fail("File is UnReacable"); }

            var extention = Path.GetExtension(fileName);

            if (string.IsNullOrWhiteSpace(extention) || !_allowedExtention.Contains(extention)) {
               
                    _logger.LogError($"File Rejected : Extension{extention} not Allowed");
                   return Result<string?>.Fail("Extention is not Allowed");
            }

            var uploadsfolder = Path.Combine(_webHost.ContentRootPath, folderName);
            Directory.CreateDirectory(uploadsfolder);

            var storedFileName = $"{Guid.NewGuid()}{fileName}";
            var filePath = Path.Combine(uploadsfolder, storedFileName);

            try
            {
                using var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write);
                await filestream.CopyToAsync(fs, ct);
                return Result<string?>.Ok(storedFileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed To upload File {fileName}");
                return Result<string?>.Fail("Failed To upload file");
            }
        }
    }
}
