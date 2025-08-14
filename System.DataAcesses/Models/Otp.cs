using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.DataAcesses.Data.Enums;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.DataAcesses.Models
{
    public class Otp
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid CompanyId { get; set; }

        public Company Company { get; set; } = default!;

        [Required]
        public string Code { get; set; } = default!;

        public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;

        public DateTime ExpiryTime { get; set; } 

        public bool IsUsed { get; set; } = false;

        public OtpType Type { get; set; } = OtpType.EmailVerification; 
    }
}
