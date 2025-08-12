using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.DataAcesses.Exceptions
{
    public class EmailNotFoundException(string email) : NotFoundException($"Email {email} is not found or not confirmed")
    {
    }
}
