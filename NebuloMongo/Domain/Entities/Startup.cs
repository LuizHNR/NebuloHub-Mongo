using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using NebuloMongo.Domain.Enum;

namespace NebuloMongo.Domain.Entities
{
    public class Startup
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("cnpj")]
        public string CNPJ { get; set; }
        [BsonElement("video")]
        public string Video {  get; set; }
        [BsonElement("name")]
        public string Name { get; set; }
        [BsonElement("descricao")]
        public string Descricao { get; set; }
        [BsonElement("site")]
        public string Site { get; set; }
        [BsonElement("date")]
        public DateTime DataCriacao { get; set; } = DateTime.UtcNow.Date;
        [BsonElement("habilidades")]
        public List<string> Habilidades { get; set; } = new();
        [BsonElement("IdUser")]
        public string IdUser { get; set; }

        private Startup(string cnpj, string video, string name, string descricao, string site, DateTime dataCriacao, List<string> habilidades, string idUser)
        {
            CNPJ = cnpj;
            Video = video;
            Name = name;
            Descricao = descricao;
            Site = site;
            DataCriacao = dataCriacao;
            Habilidades = habilidades;
            IdUser = idUser;

        }

        public void Atualizar(string cnpj, string video, string name, string descricao, string site, DateTime dataCriacao, List<string> habilidades, string idUser)
        {
            CNPJ = cnpj;
            Video = video;
            Name = name;
            Descricao = descricao;
            Site = site;
            DataCriacao = dataCriacao;
            Habilidades = habilidades;
            IdUser = idUser;
        }

        internal static Startup Create(string cnpj, string video, string name, string descricao, string site, DateTime dataCriacao, List<string> habilidades, string idUser)
        {
            return new Startup(cnpj, video, name, descricao, site, dataCriacao, habilidades, idUser);
        }

        public Startup() { }
    }
}
