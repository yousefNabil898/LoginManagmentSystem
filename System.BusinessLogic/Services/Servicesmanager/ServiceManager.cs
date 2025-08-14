using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System.BusinessLogic.Services.Attachmentservices;
using System.BusinessLogic.Services.EmailSevice;
using System.Collections.Generic;
using System.DataAcesses.Data.Repositories.Company;
using System.DataAcesses.Models;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.BusinessLogic.Services.Servicesmanager
{
    public class ServiceManager(ICompanyRepository _companyRepository, IEmailService _emailService, IAttachmentService _attachmentService, IPasswordHasher<Company> _passwordHasher, IConfiguration _configuration, IOptions<SmtpSettings> _options) : IServiceManager
    {
        private readonly Lazy<IEmailService> _emailService = new Lazy<IEmailService>(() => new EmailService(_options));
        public IEmailService EmailService => _emailService.Value;
        private readonly Lazy<IAttachmentService> _attachmentService = new Lazy<IAttachmentService>(() => new AttachmentService());
        public IAttachmentService AttachmentService => _attachmentService.Value;

    }
}
