using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.DataAcesses.Models;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.BusinessLogic.InterFaces
{
    public interface IEmailService
    {
      
        Task SendEmailAsync(string toEmail, string subject, string body);

    }
}
