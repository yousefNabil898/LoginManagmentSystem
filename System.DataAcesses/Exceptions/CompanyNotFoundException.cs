using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.DataAcesses.Exceptions
{
    public class CompanyNotFoundException(Guid id) : NotFoundException($"company with id {id} is not found")
    {
    }
}
