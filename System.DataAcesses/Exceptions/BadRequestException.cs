using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.DataAcesses.Exceptions
{
    public class BadRequestException(List<string> errors) : Exception("Validation Failed !")
    {
        public List<string> Errors { get; } = errors;
    }
}
