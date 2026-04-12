namespace RecipesAPI.Models
{
    public class IngredienteRecetaDto
    {
        public string NombreIngrediente { get; set; } = null!;
        public int Cantidad { get; set; }
        public string UnidadMedida { get; set; } = null!;
    }
}
