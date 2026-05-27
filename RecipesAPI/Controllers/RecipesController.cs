using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecipesAPI.Models;
using RecipesAPI.Services;

namespace RecipesAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class RecipesController : ControllerBase
    {
        private readonly IRecetaService _irecetaService;
        public RecipesController(IRecetaService irecetaservice)
        {
            _irecetaService = irecetaservice;
        }

        [HttpGet]
        [Route("listarRecetas")]
        public async Task<IActionResult> ListarRecetas()
        {
            try
            {
                var recetas = await _irecetaService.AllRecipes();
                return Ok(recetas);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "ocurrió un error", error = ex.Message });
            }

        }

        [Authorize]
        [HttpPost]
        [Route("agregarReceta")]
        public async Task<IActionResult> AgregarReceta([FromForm] RecetaCreateDto receta, IFormFile? file)
        {
            try
            {
                var recetaAgregada = await _irecetaService.AddRecipe(receta, file);
                return Ok(recetaAgregada);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Ocurrió un error", error = ex.Message });
            }

        }

        [Authorize]
        [HttpPost]
        [Route("recetasPorIngrediente")]
        public async Task<IActionResult> BuscarRecetas([FromBody] List<int> idIngredientes)
        {
            try
            {
                var recetasEncontradas = await _irecetaService.RecipeByIngredient(idIngredientes);
                return Ok(recetasEncontradas);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Ocurrió un error", error = ex.Message });
            }
        }

        [Authorize]
        [HttpPost]
        [Route("recetasAleatorio")]
        public async Task<IActionResult> RecetasAleatorias([FromBody] int cantidad)
        {
            try
            {
                var recetasAleatorias = await _irecetaService.GetRandomRecipes(cantidad);
                return Ok(recetasAleatorias);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Ocurrió un error", error = ex.Message });
            }
        }

        [Authorize]
        [HttpPost]
        [Route("agregarRecetasAGuardadas")]
        public async Task<IActionResult> GuardarRecetasFav([FromBody] RecetaGuardadaUsuario recetaGuardadaDto)
        {
            try
            {
                var guardarRecetaFav = await _irecetaService.AddRecipeToSaved(recetaGuardadaDto);
                return Ok(guardarRecetaFav);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException?.Message);
                return BadRequest(new { Message = "Ocurrió un error", Error = ex.Message });
            }
        }

        [Authorize]
        [HttpPost]
        [Route("obtenerRecetaGuardadaporIdUser")]
        public async Task<IActionResult> ObtenerRecetaGuardada([FromBody] int idReceta)
        {
            try
            {
                var receta = await _irecetaService.GetSavedRecipeByUser(idReceta);
                return Ok(receta);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Ocurrió un error", error = ex.Message });
            }
        }

        [Authorize]
        [HttpDelete]
        [Route("eliminarRecetaGuardada")]
        public async Task<IActionResult> EliminarRecetaGuardada(int idReceta)
        {
            try
            {
                var recetaDeleted = await _irecetaService.RemoveRecipeFromSaved(idReceta);
                return Ok(recetaDeleted);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = "Ocurrió un error", error = ex.Message });
            }
        }

        [Authorize]
        [HttpGet]
        [Route("recetasGuardadasUsuario")]
        public async Task<IActionResult> ObtenerRecetasGuardadas()
        {
            try
            {
                var recetasSaved = await _irecetaService.GetSavedRecipesByUser();
                return Ok(recetasSaved);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = "Ocurrió un error", error = ex.Message });
            }
        }
    }
}
