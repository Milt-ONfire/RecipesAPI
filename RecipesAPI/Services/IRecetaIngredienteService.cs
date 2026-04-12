using RecipesAPI.Models;

namespace RecipesAPI.Services
{
    public interface IRecetaIngredienteService
    {
        Task<List<RecetaIngrediente>> AllRecipeIngredients();
        Task<int> AddRecipeIngredient(RecetaIngrediente recetaIngrediente);
        Task<int> UpdateRecipeIngredient(RecetaIngrediente recetaIngrediente);
        Task<int> RemoveRecipeIngredient(RecetaIngrediente recetaIngrediente);
    }
}
