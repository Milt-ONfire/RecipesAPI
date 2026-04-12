using RecipesAPI.Models;

namespace RecipesAPI.Services
{
    public interface IRecetaService
    {
        Task<List<Receta>> AllRecipes();
        Task<object> AddRecipe(RecetaCreateDto receta, IFormFile? imageRecipe);
        Task<int> UpdateRecipe(Receta receta);
        Task<bool> GetSavedRecipeByUser(int idRecipe);
        Task<List<Receta>> RecipeByIngredient(List<int> ingredientId);
        Task<List<Receta>> GetRandomRecipes(int cantidad);
        Task<object> AddRecipeToSaved(RecetaGuardadaUsuario recetaGuardadaDto);
        Task<List<Receta>> GetSavedRecipesByUser();
        int? GetCurrentUserId();
    }
}
