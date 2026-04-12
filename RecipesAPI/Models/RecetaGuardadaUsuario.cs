using System.Text.Json.Serialization;

namespace RecipesAPI.Models
{
    public partial class RecetaGuardadaUsuario
    {
        public int IdRecetaGuardada { get; set; }
        public int? IdUsuario { get; set; }
        public int IdReceta { get; set; }

        [JsonIgnore]
        public virtual Usuario? IdUsuarioNavigation { get; set; } = null;
        [JsonIgnore]
        public virtual Receta? IdRecetaNavigation { get; set; } = null;
    }
}