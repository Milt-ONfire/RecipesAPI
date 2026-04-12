using Microsoft.EntityFrameworkCore;
using RecipesAPI.Models;

namespace RecipesAPI.Services
{
    public class IngredienteService : IIngredienteService
    {
        private readonly RecipesDbContext _context;
        private readonly DbSet<Ingrediente> _ingrediente;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public IngredienteService(RecipesDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _ingrediente = _context.Set<Ingrediente>();
            _httpContextAccessor = httpContextAccessor;
        }

        public Task<int> AddIngredient(Ingrediente ingrediente)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Ingrediente>> AllIngredients()
        {
            return await _ingrediente.ToListAsync();
        }

        public Task<int> RemoveIngredient(Ingrediente ingrediente)
        {
            throw new NotImplementedException();
        }

        public Task<int> UpdateIngredient(Ingrediente ingrediente)
        {
            throw new NotImplementedException();
        }
    }
}
