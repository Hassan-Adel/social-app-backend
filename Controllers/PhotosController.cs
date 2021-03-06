﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SocialApp.API.Data;
using SocialApp.API.DTOs;
using SocialApp.API.Helpers;
using SocialApp.API.Models;

namespace SocialApp.API.Controllers
{
    [Route("api/users/{userId}/photos")]
    [ApiController]
    public class PhotosController : ControllerBase
    {
        private readonly ISocialRepository _repo;
        private readonly IMapper _mapper;
        private readonly IOptions<CloudinarySettings> _cloudinaryConfig;
        private Cloudinary _cloudinary;

        public PhotosController(ISocialRepository repo, IMapper mapper, IOptions<CloudinarySettings> cloudinaryConfig)
        {
            _repo = repo;
            _mapper = mapper;
            _cloudinaryConfig = cloudinaryConfig;

            Account acc = new Account(
                _cloudinaryConfig.Value.CloudName,
                _cloudinaryConfig.Value.ApiKey,
                _cloudinaryConfig.Value.ApiSecret
                );

            _cloudinary = new Cloudinary(acc);
        }
        // GET: api/Photos
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/Photos/5
        [HttpGet("{id}", Name = "GetPhoto")]
        public async Task<IActionResult> GetPhoto(int id)
        {
            var photoFromRepo = await _repo.GetPhoto(id);
            //Because we don't want to return everything from our photoFromRepo
            var photo = _mapper.Map<PhotosForReturnDTO>(photoFromRepo);
            return Ok(photo);
        }

        // POST: api/Photos
        [HttpPost]
        public async Task<IActionResult> AddPhotoForUser(int userId, [FromForm] PhotosForCreationDTO photosForCreationDTO)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            var userFromRepo = await _repo.GetUser(userId);

            var file = photosForCreationDTO.File;

            //Store result returned from cloudinary
            var uploadResults = new ImageUploadResult();

            //And if the file's length > 0 then will read this file into memory.
            //Because this is going to be a file stream we'll use using so that we can dispose of what the file inside memory
            //once we've completed this method.
            if (file.Length > 0)
            {
                using(var stream = file.OpenReadStream())
                {
                    var uploadParams = new ImageUploadParams()
                    {
                        File = new FileDescription(file.Name, stream),
                        //Crop the image arround the user's face
                        Transformation = new Transformation().Width(500).Height(500).Crop("fill").Gravity("face")
                    };
                    uploadResults = _cloudinary.Upload(uploadParams);
                }
            }
            photosForCreationDTO.Url = uploadResults.Uri.ToString();
            photosForCreationDTO.PublicId = uploadResults.PublicId;

            var photo = _mapper.Map<Photo>(photosForCreationDTO);

            //If the user doesn't have a main photo set isMain to true
            if (!userFromRepo.Photos.Any(p => p.IsMain))
                photo.IsMain = true;

            //Save photo
            userFromRepo.Photos.Add(photo);

            if (await _repo.SaveAll())
            {
                var photoToReturn = _mapper.Map<PhotosForReturnDTO>(photo);
                return CreatedAtRoute("GetPhoto", new { id = photo.Id }, photoToReturn); // Http post should return created at route (201)
            }

            return BadRequest("Could not add the photo");
        }

        // POST: Set the isMain property of the photo to true
        [HttpPost("{id}/setMain")]
        public async Task<IActionResult> SetMainPhoto(int userId, int id)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();
            var user = await _repo.GetUser(userId);

            if (!user.Photos.Any(p => p.Id == id))
                return Unauthorized();

            var photoFromRepo = await _repo.GetPhoto(id);

            if (photoFromRepo.IsMain)
                return BadRequest("This photo is already the main photo");

            //Reset the current main photo to false
            var currentMainPhoto = await _repo.GetMainPhotoForUser(userId);
            currentMainPhoto.IsMain = false;

            photoFromRepo.IsMain = true;

            if (await _repo.SaveAll())
                return NoContent();

            return BadRequest("Could not set photo to main");
        }

        // PUT: api/Photos/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePhoto(int userId, int id)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();
            var user = await _repo.GetUser(userId);

            if (!user.Photos.Any(p => p.Id == id))
                return Unauthorized();

            var photoFromRepo = await _repo.GetPhoto(id);

            if (photoFromRepo.IsMain)
                return BadRequest("You can't delete the main photo");

            //If the hoto is stored in cloudinary
            if(photoFromRepo.PublicId != null){
                //Delete photo from Cloudinary
                var deleteParams = new DeletionParams(photoFromRepo.PublicId);
                var result = _cloudinary.Destroy(deleteParams);

                if (result.Result == "ok")
                {
                    //Delete from DB
                    _repo.Delete(photoFromRepo);
                }
            }
            
            if(photoFromRepo.PublicId == null)
            {
                //Delete from DB
                _repo.Delete(photoFromRepo);
            }

            if (await _repo.SaveAll())
                return Ok();

            return BadRequest("Failed to delete the photo");
        }
    }
}
