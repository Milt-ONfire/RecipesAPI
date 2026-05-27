using Microsoft.EntityFrameworkCore;
using RecipesAPI.Models;
using System.CodeDom;
using System.Security.Claims;

namespace RecipesAPI.Services
{
    public class RecetaService : IRecetaService
    {
        private readonly RecipesDbContext _context;
        private readonly DbSet<Receta> _dbSet;
        private readonly DbSet<RecetaIngrediente> _dbSetRecetaIngrediente;
        private readonly DbSet<RecetaGuardadaUsuario> _dbSetRecetaGuardadaUsuario;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ImageService _imageService;
        public RecetaService(RecipesDbContext context, IHttpContextAccessor httpContextAccessor, ImageService imageService)
        {
            _context = context;
            _dbSet = _context.Set<Receta>();
            _dbSetRecetaIngrediente = _context.Set<RecetaIngrediente>();
            _httpContextAccessor = httpContextAccessor;
            _imageService = imageService;
            _dbSetRecetaGuardadaUsuario = _context.Set<RecetaGuardadaUsuario>();
        }

        public int? GetCurrentUserId()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user == null) return null;

            var idClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value
                       ?? user.FindFirst("nameid")?.Value; // fallback si usas claim "id"

            if (int.TryParse(idClaim, out var userId))
                return userId;

            return null;
        }

        public async Task<object> AddRecipe(RecetaCreateDto dto, IFormFile? imageRecipe)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                // manejo: si no hay usuario, puedes lanzar excepción o devolver error
                throw new InvalidOperationException("Usuario no autenticado.");
            }

            static string Normalize(string s) => s?.Trim().ToLowerInvariant() ?? string.Empty;

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var imagen = await _imageService.GuardarImagen(imageRecipe, "recetas");
                // 1) Crear la entidad Receta base
                var receta = new Receta
                {
                    IdUsuario = userId.Value,
                    NombreReceta = dto.NombreReceta,
                    Descripcion = dto.Descripcion,
                    Categoria = dto.Categoria,
                    Imagen = !string.IsNullOrEmpty(imagen) ? imagen : "",
                    IdCategoria = dto.IdCategoria
                };

                _context.Receta.Add(receta);
                // guardamos primero la receta para obtener su Id (opcional pero útil)
                await _context.SaveChangesAsync();

                if (dto.Ingredientes != null && dto.Ingredientes.Any())
                {
                    // Obtener nombres normalizados de los ingredientes entrantes
                    var incomingNames = dto.Ingredientes
                                          .Select(i => Normalize(i.NombreIngrediente))
                                          .Where(n => !string.IsNullOrEmpty(n))
                                          .Distinct()
                                          .ToList();

                    // 2) Buscar ingredientes existentes en la BD (por nombre normalizado)
                    // Nota: si tu BD no tiene columna normalizada, comparamos en memoria:
                    var existentes = await _context.Ingredientes
                        .Where(x => incomingNames.Contains(x.NombreIngrediente.Trim().ToLower()))
                        .ToListAsync();

                    // Construir un diccionario por nombre normalizado para reuse rápido
                    var existentesDict = existentes
                        .ToDictionary(x => Normalize(x.NombreIngrediente), x => x);

                    // 3) Para cada ingrediente entrante: si no existe -> crearlo; siempre crear la relación RecetaIngrediente
                    foreach (var ingDto in dto.Ingredientes)
                    {
                        var norm = Normalize(ingDto.NombreIngrediente);
                        if (string.IsNullOrEmpty(norm))
                            continue;

                        Ingrediente ingredienteEntidad;
                        if (existentesDict.TryGetValue(norm, out var found))
                        {
                            ingredienteEntidad = found;
                        }
                        else
                        {
                            // crear nuevo ingrediente
                            ingredienteEntidad = new Ingrediente
                            {
                                NombreIngrediente = ingDto.NombreIngrediente.Trim()
                            };
                            _context.Ingredientes.Add(ingredienteEntidad);
                            // agregamos al diccionario para no duplicar en este loop
                            existentesDict[norm] = ingredienteEntidad;
                        }

                        // crear relación RecetaIngrediente
                        var recetaIng = new RecetaIngrediente
                        {
                            IdReceta = receta.IdReceta,
                            IdIngrediente = ingredienteEntidad.IdIngrediente,
                            Cantidad = ingDto.Cantidad
                        };

                        receta.RecetaIngredientes.Add(recetaIng);
                        _context.RecetaIngredientes.Add(recetaIng);
                    }

                    // Guardar los nuevos ingredientes y relaciones
                    await _context.SaveChangesAsync();
                }

                await transaction.CommitAsync();

                // Recargar la receta con ingredientes e ingredientes reales
                var recetaCompleta = await _context.Receta
                    .Include(r => r.RecetaIngredientes)
                        .ThenInclude(ri => ri.IdIngredienteNavigation)
                    .FirstOrDefaultAsync(r => r.IdReceta == receta.IdReceta);
                return (new { message = "receta guardada", Data = recetaCompleta! });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return new { message = "Error", error = ex.Message };
            }

        }

        public async Task<List<Receta>> AllRecipes()
        {
            return await _dbSet.Include(r => r.RecetaIngredientes)
                                  .ThenInclude(ri => ri.IdIngredienteNavigation).ToListAsync();
        }

        public async Task<List<Receta>> RecipeByIngredient(List<int> idIngredientes)
        {
            var recipesByIngredients = await _dbSet.Where(r => r.RecetaIngredientes
                                                                .Any(ri => idIngredientes.Contains(ri.IdIngrediente)))
                                                                .Include(r => r.RecetaIngredientes)
                                                                .ThenInclude(ri => ri.IdIngredienteNavigation)
                                                                .ToListAsync();
            return recipesByIngredients;

        }

        public async Task<List<Receta>> GetRandomRecipes(int cantidad)
        {
            var total = await _dbSet.CountAsync();

            if (total == 0)
                return new List<Receta>();

            var random = new Random();
            var skip = random.Next(0, Math.Max(0, total - cantidad));

            return await _dbSet
                .Skip(skip)
                .Take(cantidad)
                .ToListAsync();
        }

        public async Task<object> AddRecipeToSaved(RecetaGuardadaUsuario recetaGuardadaDto)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                // manejo: si no hay usuario, puedes lanzar excepción o devolver error
                throw new InvalidOperationException("Usuario no autenticado.");
            }
            recetaGuardadaDto.IdUsuario = userId;
            var validarReceta = await _dbSetRecetaGuardadaUsuario
                                       .AnyAsync(rg => rg.IdReceta == recetaGuardadaDto.IdReceta && rg.IdUsuario == recetaGuardadaDto.IdUsuario);
            if (validarReceta)
            {
                return new { Message = "Ésta receta ya ha sido guardada ", Receta = validarReceta };
            }

            _dbSetRecetaGuardadaUsuario.Add(recetaGuardadaDto);
            await _context.SaveChangesAsync();
            return recetaGuardadaDto;
        }

        public async Task<object> RemoveRecipeFromSaved(int idRecipe)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                // manejo: si no hay usuario, puedes lanzar excepción o devolver error
                throw new InvalidOperationException("Usuario no autenticado.");
            }

            var encontrarReceta = await _dbSetRecetaGuardadaUsuario
                                            .Where(rg => rg.IdReceta == idRecipe && rg.IdUsuario == userId).ToListAsync();

            if (encontrarReceta == null)
            {
                return new { message = "no hay receta con id " + idRecipe + " y usuario # " + userId + " guardada" };
            }

            var recetasEliminadas = new List<RecetaGuardadaUsuario>();

            foreach (var item in encontrarReceta)
            {
                
                var recetaEliminada = _dbSetRecetaGuardadaUsuario.Remove(item);
                await _context.SaveChangesAsync();
                recetasEliminadas.Add(item);
            }
            
            return new { message = "receta eliminada", recetasEliminadas };
        }

        public async Task<bool> GetSavedRecipeByUser(int idRecipe)
        {
            var userId = GetCurrentUserId();

            var savedRecipe = await _dbSetRecetaGuardadaUsuario.AnyAsync(rg => rg.IdReceta == idRecipe && rg.IdUsuario == userId);

            if (!savedRecipe)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public async Task<List<Receta>> GetSavedRecipesByUser()
        {
            var userId = GetCurrentUserId();
            var recipesSaved = await _dbSet.Where(r => r.RecetaGuardada.Any(rs => rs.IdUsuario == userId))
                                                                .Include(r => r.RecetaIngredientes)
                                                                .ThenInclude(r => r.IdIngredienteNavigation)
                                                                .ToListAsync();
            return recipesSaved;
        }

        public Task<int> UpdateRecipe(Receta receta)
        {
            throw new NotImplementedException();
        }
    }
}
