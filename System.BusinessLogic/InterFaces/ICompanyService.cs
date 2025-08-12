using System;
using System.BusinessLogic.Dtos;
using System.Collections.Generic;
using System.DataAcesses.Models;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.BusinessLogic.InterFaces
{
    public interface ICompanyService
    {
        Task<RegisterToReturn> RegisterCompanyAsync(RegisterCompanyDto dto);
        Task<bool> VerifyOtpAsync(VerifyOtpDto dto);
        Task<bool> ResendOtpAsync(EmailDto email);
        Task<bool> SetPasswordAsync(SetPasswordDto dto);
        Task<LoginToReturn> LoginAsync(LoginDto dto);
        Task<CompanyProfileDto?> GetProfileAsync(Guid companyId);

    }
}
