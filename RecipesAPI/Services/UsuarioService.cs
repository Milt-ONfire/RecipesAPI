using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RecipesAPI.Models;
using System.Security.Claims;

namespace RecipesAPI.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly RecipesDbContext _context;
        private readonly DbSet<Usuario> _dbSet;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ImageService? _imageService;
        public UsuarioService(RecipesDbContext context, IHttpContextAccessor httpContextAccessor, ImageService? imageService)
        {
            _context = context;
            _dbSet = _context.Set<Usuario>();
            _httpContextAccessor = httpContextAccessor;
            _imageService = imageService;
        }
        public async Task<object> AddUser(Usuario usuario)
        {
            var passwordHasher = new PasswordHasher<Usuario>();
            usuario.Password = passwordHasher.HashPassword(usuario, usuario.Password);

            await _context.Usuarios.AddAsync(usuario);

            try
            {
                int filasAfectadas = await _context.SaveChangesAsync();
                return (new { message = "usuario guardado" });
            }
            catch (Exception ex)
            {
                return new { message = "Error", error = ex.Message };
            }


        }

        public async Task<List<Usuario>> AllUsers()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<object?> GetUserById(int id)
        {
            var usuarioEncontrar = await _dbSet.FindAsync(id);
            if (usuarioEncontrar == null)
            {
                return (new { Message = "usuario no encontrado", Id = id });
            }
            return usuarioEncontrar;
        }

        public async Task<object> RemoveUser(int id)
        {
            var usuarioEliminar = await _context.Usuarios.FindAsync(id);
            if (usuarioEliminar == null)
            {
                return (new { message = "usuario no encontrado" });
            }

            _context.Usuarios.Remove(usuarioEliminar);
            int filasAfectadas = await _context.SaveChangesAsync();

            return (new { message = "usuario eliminado exitosamente" });
        }

        public async Task<object> UpdateUser(Usuario usuario, IFormFile? file)
        {

            if (file != null && file.Length > 0)
            {
                string? imagen = await _imageService.GuardarImagen(file, "usuarios");
                usuario.Imagen = imagen;
            }
            if (!string.IsNullOrEmpty(usuario.Password))
            {
                var passwordHasher = new PasswordHasher<Usuario>();
                usuario.Password = passwordHasher.HashPassword(usuario, usuario.Password);
            }

            _context.Usuarios.Update(usuario);
            var usuarioActualizado = await _context.SaveChangesAsync();
            return usuarioActualizado;

        }

        public async Task<object> GetCurrentUser()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user == null) return null;

            var idClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value
                       ?? user.FindFirst("nameid")?.Value; // fallback si usas claim "id"
            var email = user.FindFirst(ClaimTypes.Email)?.Value;

            if (!int.TryParse(idClaim, out var userId))
                return null;

            return new Usuario
            {
                IdUsuario = userId,
                NombreUsuario = user.FindFirst("name")?.Value ?? "",
                Email = email ?? "",
                Imagen = user.FindFirst("profile")?.Value ?? ""
            };
        }
    }
}
