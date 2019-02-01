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
        
        // GET /id
        [HttpGet("{id}", Name = "GetMessage")]
        public async Task<IActionResult> GetMessage(int id)
        {
            if (id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            var messageFromRepo = await _repo.GetMessage(id);

            if (messageFromRepo == null)
                return NotFound();

            return Ok(messageFromRepo);
        }

        //
        [HttpPost]
        public async Task<IActionResult> CreateMessage(int userId, MessageForCreationAndReturnDTO messageForCreationDTO)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            messageForCreationDTO.SenderId = userId;
            var reccipient = await _repo.GetUser(messageForCreationDTO.RecipientId);

            if (reccipient == null)
                BadRequest("Could not find user");

            var message = _mapper.Map<Message>(messageForCreationDTO);

            _repo.Add(message);

            var messageToReturn = _mapper.Map<MessageForCreationAndReturnDTO>(message);

            if (await _repo.SaveAll())
                return CreatedAtRoute("GetMessage", new { id = message.Id }, messageToReturn);

            throw new Exception("Creating the message failedon save");

        }

    }
}
