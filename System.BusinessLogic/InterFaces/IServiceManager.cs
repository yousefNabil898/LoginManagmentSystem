using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.BusinessLogic.InterFaces
{
    public interface IServiceManager
    {
        IEmailService EmailService { get; }
        IAttachmentService AttachmentService { get; }
    }
}
