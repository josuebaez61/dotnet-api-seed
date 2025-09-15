using System.Text.Json.Serialization;

namespace CleanArchitecture.Infrastructure.Data.Seeds.Models
{
    public class ArgentinaCityDto
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("nombre")]
        public string Nombre { get; set; } = string.Empty;

        [JsonPropertyName("fuente")]
        public string Fuente { get; set; } = string.Empty;

        [JsonPropertyName("provincia")]
        public ProvinciaDto Provincia { get; set; } = new();

        [JsonPropertyName("departamento")]
        public DepartamentoDto Departamento { get; set; } = new();

        [JsonPropertyName("municipio")]
        public MunicipioDto Municipio { get; set; } = new();

        [JsonPropertyName("categoria")]
        public string Categoria { get; set; } = string.Empty;

        [JsonPropertyName("centroide")]
        public CentroideDto Centroide { get; set; } = new();
    }

    public class ProvinciaDto
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("nombre")]
        public string Nombre { get; set; } = string.Empty;
    }

    public class DepartamentoDto
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("nombre")]
        public string Nombre { get; set; } = string.Empty;
    }

    public class MunicipioDto
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("nombre")]
        public string Nombre { get; set; } = string.Empty;
    }

    public class CentroideDto
    {
        [JsonPropertyName("lon")]
        public double Lon { get; set; }

        [JsonPropertyName("lat")]
        public double Lat { get; set; }
    }
}
