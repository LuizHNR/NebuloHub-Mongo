using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using NebuloMongo.Domain.Enum;
using System.Text.Json.Serialization;

namespace NebuloMongo.Domain.Entities
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("cpf")]
        public string CPF { get; set; }

        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("email")]
        public string Email { get; set; }

        [BsonElement("password")]
        public string Password { get; set; }

        [BsonElement("role")]
        [BsonRepresentation(BsonType.String)]
        public Role Role { get; set; }

        [BsonElement("telefone")]
        public long? Telefone { get; set; }


        private User(string cpf, string name, string email, string password, Role role, long? telefone)
        {
            CPF = cpf;
            Name = name;
            Email = email;
            Password = password;
            Role = role;
            Telefone = telefone;

        }

        public void Atualizar(string name, string email, string password, Role role, long? telefone)
        {
            Name = name;
            Email = email;
            Password = password;
            Role = role;
            Telefone = telefone;
        }

        internal static User Create(string cpf, string name, string email, string password, Role role, long? telefone)
        {
            return new User(cpf, name, email, password, role, telefone);
        }

        public User() { }
    }
}
