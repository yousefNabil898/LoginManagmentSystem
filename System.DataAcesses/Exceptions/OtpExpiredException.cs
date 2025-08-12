using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.DataAcesses.Exceptions
{
    public class OtpExpiredException(string message = "Verifications code time is expired") : Exception(message)
    {
        
    }
}
