using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace Models
{
    public class Address
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonRepresentation(MongoDB.Bson.BsonType.String)]
        [JsonProperty("cep")]
        public string ZipCode { get; set; }

        [BsonRepresentation(MongoDB.Bson.BsonType.Int32)]
        [JsonProperty("numero")]
        public int Number { get; set; }

        [JsonProperty("logradouro")]
        public string Street { get; set; }

        [JsonProperty("complemento")]
        public string Complement { get; set; }

        [JsonProperty("localidade")]
        public string City { get; set; }

        [JsonProperty("uf")]
        public string State { get; set; }

        [JsonProperty("bairro")]
        public string Neighborhood { get; set; }
    }
}
