using RecipesAPI.Models;

namespace RecipesAPI.Services
{
    public interface ICalificacionService
    {
        Task<object> AddRating(Calificacion calificacion);
        Task<int> GetRatingAverageByRecipe(int recipeId);
    }
}
