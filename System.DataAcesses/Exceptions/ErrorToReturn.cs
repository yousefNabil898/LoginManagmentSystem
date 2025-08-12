using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.DataAcesses.Exceptions
{
    public class ErrorToReturn
    {
        public int statusCode { get; set; }

        public string message { get; set; }

        public List<string> Errors { get; set; }
    }
}
