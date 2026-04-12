using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecipesAPI.Services;

namespace RecipesAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class IngredientsController : ControllerBase
    {
        private readonly IIngredienteService _iingredienteService;

        public IngredientsController(IIngredienteService iingredienteService)
        {
            _iingredienteService = iingredienteService;
        }

        [HttpGet]
        [Route("listarIngredientes")]
        public async Task<IActionResult> ListarIngredientes()
        {
            try
            {
                var listaIngredientes = await _iingredienteService.AllIngredients();
                return Ok(listaIngredientes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "ocurrió un error", error = ex.Message });
            }
        }
    }
}
