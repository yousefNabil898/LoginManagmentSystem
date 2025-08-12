using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace System.BusinessLogic.Dtos
{
    public class RegisterCompanyDto
    {

        [Required(ErrorMessage = "The name of the company in Arabic is required.")]
        public string ArabicName { get; set; }

        [Required(ErrorMessage = "The name of the company in English is required.")]
        public string EnglishName { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Email is incorrect.")]
        public string Email { get; set; }

        [Phone(ErrorMessage = "The phone number is not valid.")]
        public string? Phone { get; set; }

        [Url(ErrorMessage = "The website link is not valid.")]
        public string? WebsiteUrl { get; set; }

        public IFormFile? Logo { get; set; }

    }
}
