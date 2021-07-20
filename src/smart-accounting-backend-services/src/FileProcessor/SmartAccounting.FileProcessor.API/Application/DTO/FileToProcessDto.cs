using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace SmartAccounting.FileProcessor.API.Application.DTO
{
    public class FileToProcessDto
    {
        public IList<IFormFile> Files { get; set; }
    }
}
