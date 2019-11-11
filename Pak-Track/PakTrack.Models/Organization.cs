using System;
using System.Collections.Generic;
using LiteDB;

namespace PakTrack.Models
{
    public class Organization
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonField("organization_name")]
        public string Name { get; set; }

        [BsonField("updated_at")]
        public DateTime UpdatedAt { get; set; }

        [BsonField("remember_toke")]
        public string RememberToken { get; set; }

        [BsonField("address")]
        public string Address { get; set; }

        [BsonField("users")]
        public ICollection<User> Users { get; set; }
    }
}