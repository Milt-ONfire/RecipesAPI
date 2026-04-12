using System.Text.Json.Serialization;

namespace RecipesAPI.Models;

public partial class Receta
{
    public int IdReceta { get; set; }

    public int IdUsuario { get; set; }

    public string NombreReceta { get; set; } = null!;

    public string Descripcion { get; set; } = null!;

    public int IdCategoria { get; set; }

    public string? Categoria { get; set; }

    public string? Imagen { get; set; }

    [JsonIgnore]
    public virtual Usuario IdUsuarioNavigation { get; set; } = null!;
    [JsonIgnore]
    public virtual Categoria? IdCategoriaNavigation { get; set; }

    public virtual ICollection<Calificacion> Calificaciones { get; set; } = new List<Calificacion>();
    public virtual ICollection<RecetaIngrediente> RecetaIngredientes { get; set; } = new List<RecetaIngrediente>();
    public virtual ICollection<RecetaGuardadaUsuario> RecetaGuardada { get; set; } = new List<RecetaGuardadaUsuario>();
}
