using MongoDB.Bson.Serialization.Attributes;

namespace NebuloMongo.Application.DTOs.Request
{
    public class RequestStartupDto
    {
        /// <example>12345678000199</example>
        /// <remarks>O CNPJ deve conter exatamente 14 dígitos numéricos.</remarks>
        public string CNPJ { get; set; }

        /// <example>https://www.youtube.com/watch?v=abcd1234</example>
        public string Video { get; set; }

        /// <example>Nebulo Corp</example>
        public string Name { get; set; }

        /// <remarks>Descrição breve sobre a startup. Máximo recomendado: 500 caracteres.</remarks>
        public string Descricao { get; set; }

        /// <example>https://nebulocorp.com</example>
        /// <remarks>Endereço oficial do site da startup.</remarks>
        public string Site { get; set; }

        /// <example>2025-11-22T14:30:00Z</example>
        public DateTime DataCriacao { get; set; } = DateTime.UtcNow.Date;

        /// <example> [ "IA", "Machine Learning", "Análise de Dados" ]</example>
        public List<string> Habilidades { get; set; } = new();

        /// <example>64ena91b92b122bd1acf9f77</example>
        /// <remarks>ID do usuário proprietário da startup.</remarks>
        public string IdUser { get; set; }
    }
}
