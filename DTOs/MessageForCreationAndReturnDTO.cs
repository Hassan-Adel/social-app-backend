using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SocialApp.API.DTOs
{
    public class MessageForCreationAndReturnDTO
    {
        public int SenderId { get; set; }
        public int RecipientId { get; set; }
        public string Content { get; set; }
        public DateTime MessageSent { get; set; }
        public MessageForCreationAndReturnDTO()
        {
            MessageSent = DateTime.Now;
        }
    }
}
