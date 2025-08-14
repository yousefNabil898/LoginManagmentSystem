using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.BusinessLogic.Helpers;
using System.BusinessLogic.Services.CompanyService.CompanyDtos;
using System.BusinessLogic.Services.CompanyService;
using System.BusinessLogic.Services.Servicesmanager;
using System.DataAcesses.Data.Enums;
using System.DataAcesses.Data.Repositories.Company;
using System.DataAcesses.Exceptions;
using System.DataAcesses.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

public class CompanyService : ICompanyService
{
    private readonly ICompanyRepository _companyRepository;
    private readonly UserManager<Company> _userManager;
    private readonly IConfiguration _configuration;
    private readonly IServiceManager _serviceManager;
    private readonly ILogger<CompanyService> _logger;
    private readonly IMapper _mapper;

    public CompanyService(
        ICompanyRepository companyRepository,
        UserManager<Company> userManager,
        IConfiguration configuration,
        IServiceManager serviceManager,
        ILogger<CompanyService> logger,
        IMapper mapper)
    {
        _companyRepository = companyRepository;
        _userManager = userManager;
        _configuration = configuration;
        _serviceManager = serviceManager;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<RegisterToReturn> RegisterCompanyAsync(RegisterCompanyDto dto)
    {
        try
        {
            var exists = await _companyRepository.EmailExistsAsync(dto.Email);
            if (exists)
                throw new EmailAlreadyExistsException(dto.Email);

            string? logoUrl = null;
            if (dto.Logo != null)
                logoUrl = await _serviceManager.AttachmentService.UploadAsync(dto.Logo, "Companies");

            var company = new Company
            {
                ArabicName = dto.ArabicName,
                EnglishName = dto.EnglishName,
                Email = dto.Email,
                CompanyEmail = dto.Email,
                UserName = dto.Email,
                PhoneNumber = dto.Phone,
                WebsiteUrl = dto.WebsiteUrl,
                LogoUrl = logoUrl
            };

            var result = await _userManager.CreateAsync(company);
            if (!result.Succeeded)
                throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));

            return RegisterToReturn.SuccessResult("Company registered successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in RegisterCompanyAsync");
            throw;
        }
    }

    public async Task<bool> VerifyOtpAsync(VerifyOtpDto dto)
    {
        try
        {
            var otp = await _companyRepository.GetOtpAsync(dto.Email, dto.OtpCode, OtpType.EmailVerification);
            if (otp == null || otp.IsUsed)
                throw new OtpInvalidException();
            if (otp.ExpiryTime < DateTime.UtcNow)
                throw new OtpExpiredException();

            otp.IsUsed = true;
            await _companyRepository.UpdateOtpAsync(otp);

            var company = await _userManager.FindByIdAsync(otp.CompanyId.ToString());
            if (company == null)
                throw new Exception("Company not found.");

            company.EmailConfirmed = true;
            await _userManager.UpdateAsync(company);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in VerifyOtpAsync");
            throw;
        }
    }

    public async Task<bool> SetPasswordAsync(SetPasswordDto dto)
    {
        try
        {
            if (dto.Password != dto.ConfirmPassword)
                throw new Exception("Passwords do not match.");

            var company = await _userManager.FindByEmailAsync(dto.Email);
            if (company == null || !company.EmailConfirmed)
                throw new EmailNotFoundException(dto.Email);

            if (!HelperClass.ValidatePassword(dto.Password))
                throw new Exception("Password must contain uppercase, number, and special character.");

            var token = await _userManager.GeneratePasswordResetTokenAsync(company);
            var result = await _userManager.ResetPasswordAsync(company, token, dto.Password);
            if (!result.Succeeded)
                throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in SetPasswordAsync");
            throw;
        }
    }

    public async Task<LoginToReturn?> LoginAsync(LoginDto dto)
    {
        try
        {
            var company = await _userManager.FindByEmailAsync(dto.Email);
            if (company == null || !company.EmailConfirmed)
                throw new EmailNotFoundException(dto.Email);

            var result = await _userManager.CheckPasswordAsync(company, dto.Password);
            if (!result)
                throw new UnauthorizedException();

            return new LoginToReturn
            {
                Token = GenerateToken(company),
                CompanyName = company.ArabicName,
                LogoUrl = company.LogoUrl
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in LoginAsync");
            throw;
        }
    }

    public async Task<CompanyProfileDto?> GetProfileAsync(Guid companyId)
    {
        try
        {
            var company = await _userManager.FindByIdAsync(companyId.ToString());
            if (company == null)
                throw new Exception("Company not found");

            return _mapper.Map<CompanyProfileDto>(company);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GetProfileAsync");
            throw;
        }
    }

    public async Task<bool> ResendOtpAsync(CompanyEmail companyEmail)
    {
        try
        {
            if (companyEmail == null || string.IsNullOrEmpty(companyEmail.email))
                throw new Exception("Email is required");

            var company = await _userManager.FindByEmailAsync(companyEmail.email);
            if (company == null)
                throw new EmailNotFoundException(companyEmail.email);

            var otp = new Otp
            {
                CompanyId = company.Id,
                Code = HelperClass.GenerateOtp(),
                GeneratedAt = DateTime.UtcNow,
                ExpiryTime = DateTime.UtcNow.AddMinutes(5),
                Type = OtpType.EmailVerification
            };
            await _companyRepository.AddOtpAsync(otp);

            var ResendOtpEmail = new Email()
            {
                Subject = $"Verification code for {company.EnglishName}",
                Body = $"Your verification code is <b>{otp.Code}</b> and expires in 5 minutes."
            };

            await _serviceManager.EmailService.SendEmailAsync(company.CompanyEmail, ResendOtpEmail);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in ResendOtpAsync");
            throw;
        }
    }

    public async Task<RegisterToReturn> SendRegistarionOtp(CompanyEmail companyEmail)
    {
        try
        {
            if (companyEmail == null || string.IsNullOrEmpty(companyEmail.email))
                return RegisterToReturn.FailResult("Email cannot be empty.");

            var company = await _userManager.FindByEmailAsync(companyEmail.email);
            if (company == null)
                throw new EmailNotFoundException(companyEmail.email);

            if (string.IsNullOrEmpty(company.CompanyEmail))
                throw new Exception("Company email is not set.");

            var otp = new Otp
            {
                CompanyId = company.Id,
                Code = HelperClass.GenerateOtp(),
                GeneratedAt = DateTime.UtcNow,
                ExpiryTime = DateTime.UtcNow.AddMinutes(5),
                Type = OtpType.EmailVerification
            };

            await _companyRepository.AddOtpAsync(otp);

            var RegistarionOtpEmail = new Email()
            {
                Subject = $"Verification code for {company.EnglishName}",
                Body = $"Your verification code is <b>{otp.Code}</b> and expires in 5 minutes."
            };

            await _serviceManager.EmailService.SendEmailAsync(company.CompanyEmail, RegistarionOtpEmail);

            return RegisterToReturn.SuccessResult("Otp sent to your email");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in SendRegistarionOtp");
            return RegisterToReturn.FailResult(ex.Message);
        }
    }

    private string GenerateToken(Company company)
    {
        try
        {
            var claims = new[]
            {
                new Claim("CompanyId", company.Id.ToString()),
                new Claim(ClaimTypes.Email, company.Email!),
                new Claim("CompanyName", company.EnglishName)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddDays(7),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GenerateToken");
            throw;
        }
    }
}
