using Microsoft.AspNetCore.Mvc;
using System.BusinessLogic.Dtos;
using System.BusinessLogic.InterFaces;
using System.DataAcesses.Exceptions;
using System.Security.Claims;

namespace LoginManagmentSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompanyController : ControllerBase
    {
        private readonly ICompanyService _companyService;

        public CompanyController(ICompanyService companyService)
        {
            _companyService = companyService;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> RegisterCompany([FromForm] RegisterCompanyDto companyDto)
        {
            var company = await _companyService.RegisterCompanyAsync(companyDto);
            return Ok(company);
        }
        [HttpPost("verifyOtp")]
        public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpDto dto)
        {
            var result = await _companyService.VerifyOtpAsync(dto);
            if (!result)
                throw new OtpInvalidException();

            return Ok(result);
        }
        [HttpPost("setPassword")]
        public async Task<IActionResult> SetPassword([FromBody] SetPasswordDto dto)
        {
            var result = await _companyService.SetPasswordAsync(dto);
            if (!result)

                return BadRequest(new { success = false, message = "The operation failed" });
            return Ok(result);
        }
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var result = await _companyService.LoginAsync(dto);

            if (result == null)
                return Unauthorized("Entry data is incorrect or not confirmed email.");

            return Ok(result);
        }       
        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            var companyIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(companyIdString) || !Guid.TryParse(companyIdString, out Guid companyId))
            {
                return Unauthorized("Token is invalid");
            }

            var profile = await _companyService.GetProfileAsync(companyId);
            if (profile == null)
                return NotFound("The company was not found.");

            return Ok(profile);
        }
        [HttpPost("resendOtp")]
        public async Task<IActionResult> ResendOtp([FromBody] EmailDto email)
        {
            var result = await _companyService.ResendOtpAsync(email);
            if (!result)
                return BadRequest(new {succses = false ,message = "The code could not be rescued, check the mail or you're definitely already." });

            return Ok(result);
        }
    }
}
