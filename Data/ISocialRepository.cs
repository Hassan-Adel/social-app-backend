﻿using SocialApp.API.Helpers;
using SocialApp.API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SocialApp.API.Data
{
    public interface ISocialRepository
    {
        void Add<T>(T entity) where T : class;
        void Delete<T>(T entity) where T : class;
        Task<bool> SaveAll();
        Task<PagedList<User>> GetUsers(UserParams userParams);
        Task<User> GetUser(int id);
        Task<Photo> GetPhoto(int id);
        Task<Photo> GetMainPhotoForUser(int userId);
        Task<Like> GetLike(int userId, int recipientId);
        Task<Message> GetMessage(int id);
        Task<PagedList<Message>> GetMessagesForUser();
        Task<IEnumerable<Message>> GetMessageThread(int userId, int recipientId);
    }
}
