using System;
using System.Collections.Generic;
using System.DataAcesses.Models;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.DataAcesses.Data.Repositories.InterFaces
{
    public interface ICompanyRepository
    {
        Task AddCompanyAsync(Company company);
        Task<Company?> GetByEmailAsync(string email);
        Task<Company?> GetByIdAsync(Guid id);
        Task UpdateAsync(Company company);
        Task<bool> EmailExistsAsync(string email);
    }
}
