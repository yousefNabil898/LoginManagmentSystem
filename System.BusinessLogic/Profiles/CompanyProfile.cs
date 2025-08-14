using AutoMapper;
using System;
using System.BusinessLogic.Services.CompanyService.CompanyDtos;
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
            CreateMap<Company,CompanyProfileDto>().ForMember(des => des.Phone , opt => opt.MapFrom(src => src.PhoneNumber));
        }
    }
}
