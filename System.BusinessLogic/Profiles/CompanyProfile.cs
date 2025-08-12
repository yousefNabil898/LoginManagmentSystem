using AutoMapper;
using System;
using System.BusinessLogic.Dtos;
using System.Collections.Generic;
using System.DataAcesses.Models;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.BusinessLogic.Profiles
{
    public class CompanyProfile :Profile
    {
        public CompanyProfile()
        {
            CreateMap<Company,CompanyProfileDto>();
        }
    }
}
