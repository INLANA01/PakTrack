using System.Collections.Generic;
using LiteDB;
using PakTrack.Models;

namespace PakTrack.DAL.Interfaces
{
    public interface IOrganizationRepository
    {
        Organization GetById(ObjectId eventId);
        IEnumerable<Organization> GetAll();

        Organization GetByUserName(string username);

        IEnumerable<User> GetUsersByOrganizationId(ObjectId id);

        User GetUserInformationByUsername(string username);

        bool AuthenticateUser(string userName, string password);

    }
}