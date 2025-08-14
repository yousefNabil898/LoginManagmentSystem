using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.BusinessLogic.Services.CompanyService.CompanyDtos
{
    public class VerifyOtpDto
    {
        public string Email { get; set; }
        public string OtpCode { get; set; }
    }
}
