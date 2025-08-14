using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.DataAcesses.Models;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.BusinessLogic.Services.EmailSevice
{
    public interface IEmailService
    {

        Task SendEmailAsync(string toEmail, Email email);

    }
}
