using System.BusinessLogic.Services.Attachmentservices;
using System.BusinessLogic.Services.EmailSevice;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.BusinessLogic.Services.Servicesmanager
{
    public interface IServiceManager
    {
        IEmailService EmailService { get; }
        IAttachmentService AttachmentService { get; }
    }
}
