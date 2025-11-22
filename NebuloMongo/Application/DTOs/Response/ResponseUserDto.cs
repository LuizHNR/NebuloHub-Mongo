using NebuloMongo.Domain.Entities;
using NebuloMongo.Domain.Enum;
using System.ComponentModel.DataAnnotations;

namespace NebuloMongo.Application.DTOs.Response
{
    public class ResponseUserDto
    {
        public string Id { get; set; }
        public string CPF { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        [EnumDataType(typeof(Role))]
        public Role Role { get; set; }
        public long? Telefone { get; set; }

        public static ResponseUserDto FromEntity(User u)
        {
            return new ResponseUserDto
            {
                Id = u.Id,
                CPF = u.CPF,
                Name = u.Name,
                Email = u.Email,
                Password = u.Password,
                Role = u.Role,
                Telefone = u.Telefone
            };
        }
    }
}
