using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.DataAcesses.Exceptions
{
    public class OtpInvalidException(string message = "Email or Otp is Invailed" ): Exception(message)
    {
     
    }
}
