using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.DataAcesses.Models;
using System.DataAcesses.Data.Context;
using System.DataAcesses.Data.Enums;

namespace System.DataAcesses.Data.Repositories.Company
{
    public class CompanyRepository(SystemContext _context) : ICompanyRepository
    {

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _context.Users.AnyAsync(c => c.Email == email);
        }

        public async Task AddOtpAsync(Otp otp)
        {
            await _context.Otps.AddAsync(otp);
            await _context.SaveChangesAsync();
        }

        public async Task<Otp?> GetOtpAsync(string email, string code, OtpType type)
        {
            return await _context.Otps
                .Include(o => o.Company)
                .Where(o =>
                    o.Company.Email == email &&
                    o.Code == code &&
                    o.Type == type &&
                    !o.IsUsed)
                .OrderByDescending(o => o.GeneratedAt)
                .FirstOrDefaultAsync();
        }

        public async Task UpdateOtpAsync(Otp otp)
        {
            _context.Otps.Update(otp);
            await _context.SaveChangesAsync();
        }
    }
}

