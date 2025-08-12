using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.DataAcesses.Data.Context;
using System.DataAcesses.Data.Repositories.InterFaces;
using System.DataAcesses.Models;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.DataAcesses.Data.Repositories.Classes
{
    public class CompanyRepository(SystemContext _context) : ICompanyRepository
    {
        public async Task AddCompanyAsync(Company company)
        {
            await _context.Companies.AddAsync(company);
            await _context.SaveChangesAsync();
        }

        public async Task<Company?> GetByEmailAsync(string email)
        {
            return await _context.Companies.FirstOrDefaultAsync(c => c.Email == email);
        }

        public async Task<Company?> GetByIdAsync(Guid id)
        {
            return await _context.Companies.FindAsync(id);
        }

        public async Task UpdateAsync(Company company)
        {
            _context.Companies.Update(company);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _context.Companies.AnyAsync(c => c.Email == email);
        }
    }
}

