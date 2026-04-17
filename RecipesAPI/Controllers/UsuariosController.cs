using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecipesAPI.Models;
using RecipesAPI.Services;

namespace RecipesAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private readonly IUsuarioService _usuarioService;

        public UsuariosController(IUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        [Authorize]
        [HttpGet]
        [Route("listar")]
        public async Task<IActionResult> ListarUsuarios()
        {
            var usuarios = await _usuarioService.AllUsers();
            return Ok(usuarios);
        }

        [HttpPost]
        [Route("agregar")]
        public async Task<IActionResult> AgregarUsuario([FromBody] RegisterDtoRequest usuario)
        {
            var usuarioAgregado = await _usuarioService.AddUser(usuario);
            return Ok(usuarioAgregado);
        }

        [HttpGet("buscarUsuario/{id}")]
        //[Route("buscarUsuario")]
        public async Task<IActionResult> BuscarUsuario(int id)
        {
            try
            {
                var usuarioEncontrado = await _usuarioService.GetUserById(id);
                return Ok(usuarioEncontrado);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Ocurrió un error", error = ex.Message });
            }

        }

        [Authorize]
        [HttpGet("buscarUsuarioActual")]
        //[Route("buscarUsuario")]
        public async Task<IActionResult> BuscarUsuarioActual()
        {
            try
            {
                var usuarioEncontrado = await _usuarioService.GetCurrentUser();
                return Ok(usuarioEncontrado);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Ocurrió un error", error = ex.Message });
            }

        }

        [Authorize]
        [HttpPut]
        [Route("actualizar")]
        public async Task<IActionResult> ActualizarUsuario([FromForm] Usuario usuario, IFormFile? file)
        {
            try
            {
                var usuarioActualizado = await _usuarioService.UpdateUser(usuario, file);
                var usuarioModificado = await _usuarioService.GetUserById(usuario.IdUsuario);
                return Ok(new { message = "usuario modificado exitosamente", data = usuarioModificado });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Ocurrió un error", error = ex.Message });
            }
        }

        [Authorize]
        [HttpDelete]
        [Route("eliminar")]
        public async Task<IActionResult> EliminarUsuario([FromBody] int usuario)
        {
            try
            {
                var usuarioEliminado = await _usuarioService.RemoveUser(usuario);
                return Ok(usuarioEliminado);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Ocurrió un error", error = ex.Message });
            }
        }

    }
}
