using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialApp.API.Data;
using SocialApp.API.DTOs;
using SocialApp.API.Helpers;
using SocialApp.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SocialApp.API.Controllers
{
    [ServiceFilter(typeof(LogUserActivity))]
    [Authorize]
    [Route("api/users/{userId}/[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private readonly ISocialRepository _repo;
        private readonly IMapper _mapper;

        public MessagesController(ISocialRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        
    }
}
