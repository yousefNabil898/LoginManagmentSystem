using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.BusinessLogic.Dtos
{
    public class CompanyProfileDto
    {
        public Guid Id { get; set; }
        public string ArabicName { get; set; }
        public string EnglishName { get; set; }
        public string Email { get; set; }
        public string? Phone { get; set; }
        public string? WebsiteUrl { get; set; }
        public string? LogoUrl { get; set; }
    }
}
