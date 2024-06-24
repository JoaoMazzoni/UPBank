using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTO
{
    public class AddressDTO
    {
        [BsonId]
        public static string Id;
        public string Complement { get; set; }
        public int Number { get; set; }
        public string ZipCode { get; set; }

        public static string CreateId (int number, string zipCode)
        {
           return Id = zipCode + number;
        }
    }
}
