using System.BusinessLogic.Services.CompanyService.CompanyDtos;
using System.Collections.Generic;
using System.DataAcesses.Models;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.BusinessLogic.Services.CompanyService
{
    public interface ICompanyService
    {
        Task<RegisterToReturn> RegisterCompanyAsync(RegisterCompanyDto dto);
        Task<RegisterToReturn> SendRegistarionOtp(CompanyEmail companyEmail);
        Task<bool> VerifyOtpAsync(VerifyOtpDto dto);
        Task<bool> ResendOtpAsync(CompanyEmail email);
        Task<bool> SetPasswordAsync(SetPasswordDto dto);
        Task<LoginToReturn> LoginAsync(LoginDto dto);
        Task<CompanyProfileDto?> GetProfileAsync(Guid companyId);



    }
}
