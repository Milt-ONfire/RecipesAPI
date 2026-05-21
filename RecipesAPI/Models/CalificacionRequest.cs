using System.Text.Json.Serialization;

namespace RecipesAPI.Models
{
    public class CalificacionRequest
    {
        public int IdCalificacion { get; set; }
        public int IdReceta { get; set; }
        public int IdUsuario { get; set; }
        public int Rating { get; set; }
        public DateTime? FechaCalificacion { get; set; } = DateTime.UtcNow;
        public string Comentarios { get; set; } = "";
        [JsonIgnore]
        public virtual Usuario? Usuario { get; set; }
        [JsonIgnore(Condition = (JsonIgnoreCondition.WhenWriting))]
        public virtual Receta? Receta { get; set; }
    }
}
