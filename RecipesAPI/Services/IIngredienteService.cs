using RecipesAPI.Models;

namespace RecipesAPI.Services
{
    public interface IIngredienteService
    {
        Task<List<Ingrediente>> AllIngredients();
        Task<int> AddIngredient(Ingrediente ingrediente);
        Task<int> UpdateIngredient(Ingrediente ingrediente);
        Task<int> RemoveIngredient(Ingrediente ingrediente);
    }
}
