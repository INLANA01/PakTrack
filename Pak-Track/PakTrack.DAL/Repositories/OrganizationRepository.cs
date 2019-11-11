using System.Collections.Generic;
using System.Linq;
using LiteDB;
using PakTrack.DAL.Interfaces;
using PakTrack.Models;

namespace PakTrack.DAL.Repositories
{
    /// <summary>
    /// This repository manages both organization and user information. It handles the basic
    /// Authentication of user
    /// </summary>
    public class OrganizationRepository : IOrganizationRepository
    {
        private readonly IPakTrackDbContext _dbContext;

        public OrganizationRepository(IPakTrackDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Get an organization given the id
        /// </summary>
        /// <param name="eventId">ObjectId of the organization</param>
        /// <returns>Organization</returns>
        public Organization GetById(ObjectId eventId)
        {
            return _dbContext.Organizations.FindById(eventId);
        }

        /// <summary>
        /// Get all the organization available in the database
        /// </summary>
        /// <returns>IEnumerable<Organization></returns>
        public IEnumerable<Organization> GetAll()
        {
            return _dbContext.Organizations.FindAll();
        }

        /// <summary>
        /// Get an organization by username
        /// </summary>
        /// <param name="username">username of the user</param>
        /// <returns>Organization</returns>
        public Organization GetByUserName(string username)
        {
            return _dbContext.Organizations.FindAll().FirstOrDefault(t => t.Users.Any(d => d.UserName == username));
        }

        /// <summary>
        /// Get Users register in a given organization
        /// </summary>
        /// <param name="id">Id of the organization</param>
        /// <returns>IEnumerable<User></returns>
        public IEnumerable<User> GetUsersByOrganizationId(ObjectId id)
        {
            return _dbContext.Organizations.FindById(id).Users;
        }


        /// <summary>
        /// Get user information for a given user's username
        /// </summary>
        /// <param name="userName">Username of the user</param>
        /// <returns>User</returns>
        public User GetUserInformationByUsername(string userName)
        {
            var allUsers = _dbContext.Organizations.FindAll().Select(t=>t.Users).FirstOrDefault();
            return allUsers?.FirstOrDefault(t => t.UserName == userName);
        }

        /// <summary>
        /// Authenticate an user
        /// </summary>
        /// <param name="userName">User's username</param>
        /// <param name="password">User's password</param>
        /// <returns>Boolean</returns>
        public bool AuthenticateUser(string userName, string password)
        {
            var allUsers = _dbContext.Organizations.FindAll().Select(t => t.Users).FirstOrDefault();
            return allUsers != null && allUsers.Any(t => t.UserName == userName && t.Password == password);
        }
    }
}