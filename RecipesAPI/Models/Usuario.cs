using System.Text.Json.Serialization;

namespace RecipesAPI.Models;

public partial class Usuario
{
    public int IdUsuario { get; set; }

    public string NombreUsuario { get; set; } = null!;

    public string? Email { get; set; }

    public string? Imagen { get; set; }

    public bool isGoogle { get; set; }

    public string? Password { get; set; } = "";

    [JsonIgnore]
    public virtual ICollection<Receta> Receta { get; set; } = new List<Receta>();
    [JsonIgnore]
    public virtual ICollection<RecetaGuardadaUsuario> RecetaGuardada { get; set; } = new List<RecetaGuardadaUsuario>();
    [JsonIgnore]
    public virtual ICollection<Calificacion> Calificaciones { get; set; } = new List<Calificacion>();
}
