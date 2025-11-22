using NebuloMongo.Domain.Enum;
using System.ComponentModel.DataAnnotations;

namespace NebuloMongo.Application.DTOs.Request
{
    public class RequestUserDto
    {
        public string CPF { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        [EnumDataType(typeof(Role))]
        public Role Role { get; set; }
        public long? Telefone { get; set; }
    }
}
