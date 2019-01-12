using AutoMapper;
using SocialApp.API.DTOs;
using SocialApp.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SocialApp.API.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap <User, UserForDetailedDTO>();
            CreateMap<User, UserForListDTO>();
        }
    }
}
