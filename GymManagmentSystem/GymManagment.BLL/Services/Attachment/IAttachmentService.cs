using Azure.Core;
using GymManagment.BLL.Common;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagment.BLL.Services.Attachment
{
    public interface IAttachmentService
    {
        Task<Result<String?>> UploadAsync(Stream filestream, string fileName, string folderName, CancellationToken ct = default);

        Result Delete(string fileName, string folderName);
        (Stream stream,string ContentType)? GetFile(string fileName, string folderName);
    }
}
