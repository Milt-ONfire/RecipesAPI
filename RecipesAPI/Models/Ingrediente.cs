using System.Text.Json.Serialization;

namespace RecipesAPI.Models;

public partial class Ingrediente
{
    public int IdIngrediente { get; set; }

    public string NombreIngrediente { get; set; } = null!;

    public string? UnidadMedida { get; set; }
    [JsonIgnore]
    public virtual ICollection<RecetaIngrediente> RecetaIngredientes { get; set; } = new List<RecetaIngrediente>();
}
