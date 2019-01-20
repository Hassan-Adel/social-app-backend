using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SocialApp.API.DTOs
{
    public class PhotosForCreationDTO
    {
        public string Url { get; set; }
        public IFormFile File { get; set; }
        public string Description { get; set; }
        public DateTime DateAdded { get; set; }
        // Cloudinary PublicId
        public string PublicId { get; set; }
        public PhotosForCreationDTO()
        {
            DateAdded = DateTime.Now;
        }
    }
}
