namespace RecipesAPI.Models
{
    public class RecetaCreateDto
    {
        public int IdUsuario { get; set; }
        public string NombreReceta { get; set; } = null!;
        public string Descripcion { get; set; } = null!;
        public string? Categoria { get; set; }
        public int IdCategoria { get; set; }
        public List<IngredienteRecetaDto>? Ingredientes { get; set; }
    }
}
