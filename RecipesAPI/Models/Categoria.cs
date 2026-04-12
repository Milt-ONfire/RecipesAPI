namespace RecipesAPI.Models
{
    public class Categoria
    {
        public int IdCategoría { get; set; }
        public string NombreCategoria { get; set; } = null!;
        public string? ImagenCategoria { get; set; }
        public virtual ICollection<Receta> Receta { get; set; } = new List<Receta>();

    }
}
