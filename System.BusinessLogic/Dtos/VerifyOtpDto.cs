using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.BusinessLogic.Dtos
{
    public class VerifyOtpDto
    {
        public string Email { get; set; }
        public string OtpCode { get; set; }
    }
}
