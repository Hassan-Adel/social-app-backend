using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SocialApp.API.Models
{
    public class Like
    {
        // Id of the user who likes
        public int LikerId { get; set; }
        // Id of the user who is liked
        public int LikeeId { get; set; }
        public User Liker { get; set; }
        public User Likee { get; set; }
    }
}
