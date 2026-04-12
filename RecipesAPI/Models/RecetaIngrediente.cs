using System.Text.Json.Serialization;

namespace RecipesAPI.Models;

public partial class RecetaIngrediente
{
    public int IdRecetaIngrediente { get; set; }

    public int IdReceta { get; set; }

    public int IdIngrediente { get; set; }

    public int Cantidad { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public virtual Ingrediente IdIngredienteNavigation { get; set; } = null!;
    [JsonIgnore]
    public virtual Receta IdRecetaNavigation { get; set; } = null!;
}
