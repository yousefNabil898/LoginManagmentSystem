using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace System.DataAcesses.Models
{
    public class Company : IdentityUser<Guid> 
    {
        [Required]
        public string ArabicName { get; set; } = default!;

        [Required]
        public string EnglishName { get; set; } = default!;

        public string? WebsiteUrl { get; set; }
        public string? LogoUrl { get; set; }
        public string? CompanyEmail { get; set; }


        public ICollection<Otp> Otps { get; set; } = new List<Otp>();
    }
}
