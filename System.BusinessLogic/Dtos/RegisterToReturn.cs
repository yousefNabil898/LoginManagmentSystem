using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.BusinessLogic.Dtos
{
    public class RegisterToReturn
    {
        public bool Success { get; set; }
        public string? Message { get; set; }

        public static RegisterToReturn SuccessResult(string? msg = null) => new RegisterToReturn { Success = true, Message = msg };
        public static RegisterToReturn FailResult(string msg) => new RegisterToReturn { Success = false, Message = msg };
    }
}
