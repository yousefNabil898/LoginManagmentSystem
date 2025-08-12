using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.DataAcesses.Exceptions
{
    public sealed class UnauthorizedException(string message = "Invailed Email or Password") : Exception(message)
    {

    }
}
