using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.BusinessLogic.InterFaces
{
    public interface IAttachmentService
    {
        Task<string> UploadAsync(IFormFile file, string folder);
    }
}
