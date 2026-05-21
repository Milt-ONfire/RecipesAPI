using RecipesAPI.Models;

namespace RecipesAPI.Services
{
    public interface ICalificacionService
    {
        Task<object> AddRating(CalificacionRequest calificacion);
        Task<int> GetRatingAverageByRecipe(int recipeId);
        Task<object> GetRatingsByrecipeId(int recipeId);
    }
}
