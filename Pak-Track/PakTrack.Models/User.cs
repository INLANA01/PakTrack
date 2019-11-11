using System;
using LiteDB;

namespace PakTrack.Models
{
    public class User
    {
        [BsonField("username")]
        public string UserName { get; set; }

        [BsonField("first_name")]
        public string FirstName { get; set; }

        [BsonField("last_name")]
        public string LastName { get; set; }

        [BsonField("email")]
        public string Email { get; set; }

        [BsonField("password")]
        public string Password { get; set; }

        [BsonField("created_at")]
        public DateTime CreatedAd { get; set; }

        [BsonField("updated_at")]
        public DateTime? UpdatedAt { get; set; }

        [BsonField("user_type")]
        public UserType UserType { get; set; }
    }
}