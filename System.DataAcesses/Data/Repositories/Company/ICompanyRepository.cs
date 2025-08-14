using System.Collections.Generic;
using System.DataAcesses.Data.Enums;
using System.DataAcesses.Models;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.DataAcesses.Data.Repositories.Company
{
    public interface ICompanyRepository
    {
        Task<bool> EmailExistsAsync(string email);

        Task AddOtpAsync(Otp otp);
        Task<Otp?> GetOtpAsync(string email, string code, OtpType type);
        Task UpdateOtpAsync(Otp otp);
    }
}
