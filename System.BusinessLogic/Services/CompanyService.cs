using System;
using System.BusinessLogic.Dtos;
using System.BusinessLogic.InterFaces;
using System.Collections.Generic;
using System.DataAcesses.Data.Context;
using System.DataAcesses.Data.Repositories.InterFaces;
using System.DataAcesses.Models;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.BusinessLogic.Helpers;
using System.DataAcesses.Data.Repositories.Classes;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;
using System.DataAcesses.Exceptions;
using AutoMapper;

namespace System.BusinessLogic.Services
{
    public class CompanyService : ICompanyService
    {
        private readonly ICompanyRepository _companyRepository;
        private readonly IPasswordHasher<Company> _passwordHasher;
        private readonly IConfiguration _configuration;
        private readonly IServiceManager _serviceManager;
        private readonly IMapper _mapper;

        public CompanyService(
            ICompanyRepository companyRepository,
            IPasswordHasher<Company> passwordHasher,
            IConfiguration config,
            IServiceManager serviceManager,
            IMapper mapper
            )
        {
            _companyRepository = companyRepository;
            _passwordHasher = passwordHasher;
            _configuration = config;
            _serviceManager = serviceManager;
            _mapper = mapper;
        }
        public async Task<RegisterToReturn> RegisterCompanyAsync(RegisterCompanyDto CompanyDto)
        {
            var IsCompanyExists = await _companyRepository.EmailExistsAsync(CompanyDto.Email);
            if (IsCompanyExists)
            {
                throw new EmailAlreadyExistsException(CompanyDto.Email);
            }
            else
            {
                string? logoPath = null;
                if (CompanyDto.Logo != null)
                {
                    logoPath = await _serviceManager.AttachmentService.UploadAsync(CompanyDto.Logo, "Companies");
                }
                var company = new Company
                {
                    ArabicName = CompanyDto.ArabicName,
                    EnglishName = CompanyDto.EnglishName,
                    Email = CompanyDto.Email,
                    Phone = CompanyDto.Phone,
                    WebsiteUrl = CompanyDto.WebsiteUrl,
                    IsEmailConfirmed = false,
                    LogoUrl = logoPath,
                    OtpCode = HelperClass.GenerateOtp(),
                    OtpGeneratedAt = DateTime.UtcNow,
                };


                await _companyRepository.AddCompanyAsync(company);

                var subject = $"The verification code of {company.EnglishName} company ";
                var body = $"The verification code is : <b>{company.OtpCode} and will exspire in 5 minutes</b> ";

                await _serviceManager.EmailService.SendEmailAsync(company.Email, subject, body);
                return RegisterToReturn.SuccessResult("The registration was successfully. The verification code has been sent to your email.");
            }
        }

        public async Task<bool> VerifyOtpAsync(VerifyOtpDto dto)
        {
            var company = await _companyRepository.GetByEmailAsync(dto.Email);
            if (company == null || company.OtpCode != dto.OtpCode)
            {
                throw new OtpInvalidException();
            }
            else
            {
                var OTPTime = DateTime.UtcNow - company.OtpGeneratedAt!.Value;
                if (OTPTime > HelperClass.OtpValidationPeriod)
                {
                    company.OtpCode = null;
                    company.OtpGeneratedAt = null;
                    await _companyRepository.UpdateAsync(company);
                    throw new OtpExpiredException();
                }
                company.IsEmailConfirmed = true;
                company.OtpCode = null;
                company.OtpGeneratedAt = null;

                await _companyRepository.UpdateAsync(company);
                return true;
            }

        }
        public async Task<bool> SetPasswordAsync(SetPasswordDto dto)
        {
            if (dto.Password != dto.ConfirmPassword)
                throw new Exception("Password and ConfirmPassword is not matched ");

            var company = await _companyRepository.GetByEmailAsync(dto.Email);
            if (company == null || !company.IsEmailConfirmed)
                throw new EmailNotFoundException(dto.Email);

            if (!HelperClass.ValidatePassword(dto.Password))
                throw new Exception("Password must contains at least one capetal letter , special character and number "); ;

            company.PasswordHash = _passwordHasher.HashPassword(company, dto.Password);

            await _companyRepository.UpdateAsync(company);
            return true;
        }

        public async Task<LoginToReturn?> LoginAsync(LoginDto dto)
        {
            var company = await _companyRepository.GetByEmailAsync(dto.Email);
            if (company == null || !company.IsEmailConfirmed)
                throw new EmailNotFoundException(dto.Email);


            var result = _passwordHasher.VerifyHashedPassword(company, company.PasswordHash!, dto.Password);

            if (result == PasswordVerificationResult.Success)
            {

                return new LoginToReturn
                {
                    Token = GenrateToken(company, _configuration),
                    CompanyName = company.ArabicName,
                    LogoUrl = company.LogoUrl
                };
            }

            throw new UnauthorizedException();
        }
        public async Task<CompanyProfileDto?> GetProfileAsync(Guid companyId)
        {
            var company = await _companyRepository.GetByIdAsync(companyId);
            if (company == null)
                throw new CompanyNotFoundException(companyId);
            var CompanyToReturn = _mapper.Map<CompanyProfileDto>(company);
            return CompanyToReturn;
        }


        public async Task<bool> ResendOtpAsync(EmailDto email)
        {
            var company = await _companyRepository.GetByEmailAsync(email.email);

            if (company == null)
                throw new EmailNotFoundException(email.email);

            if (company.OtpGeneratedAt.HasValue)
            {
                var timeSinceLastOtp = DateTime.UtcNow - company.OtpGeneratedAt.Value;
                if (timeSinceLastOtp < HelperClass.OtpValidationPeriod)
                {
                    throw new Exception("Please wait before ordering a new verification code.");
                }
            }

            company.OtpCode = HelperClass.GenerateOtp();
            company.OtpGeneratedAt = DateTime.UtcNow;
            await _companyRepository.UpdateAsync(company);

            var subject = $"Verification code of {company.EnglishName} company ";
            var body = $"the verification code is : <b>{company.OtpCode}</b> and will exspire in 5 minutes";

            await _serviceManager.EmailService.SendEmailAsync(company.Email, subject, body);

            return true;
        }
        private string GenrateToken(Company company, IConfiguration _configuration)
        {
            var claims = new[]
         {
            new Claim(ClaimTypes.NameIdentifier, company.Id.ToString()),
            new Claim(ClaimTypes.Email, company.Email),
            new Claim("CompanyName", company.EnglishName),
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
    }
}
