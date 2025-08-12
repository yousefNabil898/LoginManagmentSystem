using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.DataAcesses.Models;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace System.BusinessLogic.Helpers
{
    public static class HelperClass
    {

        public static readonly TimeSpan OtpValidationPeriod = TimeSpan.FromMinutes(5);

        public static string GenerateOtp()
        {
            var rng = RandomNumberGenerator.Create();
            var bytes = new byte[4];
            rng.GetBytes(bytes);
            int otp = BitConverter.ToInt32(bytes, 0);
            otp = Math.Abs(otp % 1000000);
            return otp.ToString("D6");
        }
        public static bool ValidatePassword(string password)
        {
            if (password.Length < 6) return false;
            if (!System.Text.RegularExpressions.Regex.IsMatch(password, "[A-Z]")) return false;
            if (!System.Text.RegularExpressions.Regex.IsMatch(password, "[0-9]")) return false;
            if (!System.Text.RegularExpressions.Regex.IsMatch(password, "[^a-zA-Z0-9]")) return false;
            return true;
        }
       
    }
}
