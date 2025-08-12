using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.DataAcesses.Models
{
    public class Company
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public string ArabicName { get; set; } = default!;

        [Required]
        public string EnglishName { get; set; } = default!;

        [Required, EmailAddress]
        public string Email { get; set; } = default!;

        public string? Phone { get; set; }

        public string? WebsiteUrl { get; set; }

        public string? LogoUrl { get; set; }

        public string? PasswordHash { get; set; }

        public bool IsEmailConfirmed { get; set; } = false;

        public string? OtpCode { get; set; }

        public DateTime? OtpGeneratedAt { get; set; }
    }
}
