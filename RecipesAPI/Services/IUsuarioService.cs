using RecipesAPI.Models;

namespace RecipesAPI.Services
{
    public interface IUsuarioService
    {
        Task<List<Usuario>> AllUsers();
        Task<object> AddUser(RegisterDtoRequest usuario);
        Task<object> UpdateUser(Usuario usuario, IFormFile? file);
        Task<object> RemoveUser(int id);
        Task<object> GetUserById(int id);
        Task<object> GetCurrentUser();
    }
}
