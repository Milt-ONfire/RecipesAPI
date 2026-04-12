using Microsoft.EntityFrameworkCore;
using RecipesAPI.Models;

namespace RecipesAPI.Services
{
    public class CalificacionService : ICalificacionService
    {
        private readonly RecipesDbContext _context;
        private readonly DbSet<Calificacion> _dbSet;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IRecetaService? _irecetaService;

        public CalificacionService(RecipesDbContext context, IHttpContextAccessor httpContextAccessor, IRecetaService? irecetaService)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _dbSet = _context.Set<Calificacion>();
            _irecetaService = irecetaService;
        }

        public async Task<object> AddRating(Calificacion rating)
        {
            var idUser = _irecetaService?.GetCurrentUserId();

            if (idUser == null)
            {
                return new { message = "Debe tener usuario activo para calificar recetas" };
            }

            rating.IdUsuario = (int)idUser;

            if (rating.IdReceta <= 0)
            {
                return new { message = "Debe enviar una receta válida" };
            }

            var alreadyRating = await _dbSet
                .AnyAsync(r => r.IdUsuario == rating.IdUsuario && r.IdReceta == rating.IdReceta);

            if (alreadyRating)
            {
                return new { message = "Este usuario ya calificó esta receta" };
            }

            var calificacion = new Calificacion
            {
                IdUsuario = (int)idUser,
                IdReceta = rating.IdReceta,
                Rating = rating.Rating,
                Comentarios = rating.Comentarios == "string" ? "" : rating.Comentarios
            };

            await _dbSet.AddAsync(calificacion);
            await _context.SaveChangesAsync();

            return calificacion;
        }

        public async Task<int> GetRatingAverageByRecipe(int recipeId)
        {
            var avg = await _dbSet.Where(c => c.IdReceta == recipeId).Select(r => (double?)r.Rating).AverageAsync() ?? 0;
            int avgEnd = (int)Math.Round(avg);
            return avgEnd;
        }

    }
}
