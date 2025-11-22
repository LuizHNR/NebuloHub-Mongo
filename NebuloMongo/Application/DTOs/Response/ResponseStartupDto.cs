using NebuloMongo.Domain.Entities;

namespace NebuloMongo.Application.DTOs.Response
{
    public class ResponseStartupDto
    {
        public string Id { get; set; }

        public string CNPJ { get; set; }
        public string Video { get; set; }
        public string Name { get; set; }
        public string Descricao { get; set; }
        public string Site { get; set; }
        public DateTime DataCriacao { get; set; } = DateTime.UtcNow;
        public List<string> Habilidades { get; set; } = new();
        public string IdUser { get; set; }

        public static ResponseStartupDto FromEntity(Startup u)
        {
            return new ResponseStartupDto
            {
                Id = u.Id,
                CNPJ = u.CNPJ,
                Video = u.Video,
                Name = u.Name,
                Descricao = u.Descricao,
                Site = u.Site,
                DataCriacao = u.DataCriacao,
                Habilidades = u.Habilidades,
                IdUser = u.IdUser
            };
        }
    }
}
