using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecipesAPI.Models;
using RecipesAPI.Services;

namespace RecipesAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RatingController : ControllerBase
    {
        private readonly ICalificacionService _icalificacionService;
        public RatingController(ICalificacionService icalificacionService)
        {
            _icalificacionService = icalificacionService;
        }

        [Authorize]
        [HttpPost]
        [Route("addRating")]
        public async Task<IActionResult> addRating([FromBody] CalificacionRequest rating)
        {
            try
            {
                var newRating = await _icalificacionService.AddRating(rating);
                return Ok(new { message = "calificación creada", data = newRating });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Ocurrió un error", error = ex.Message });
            }
        }

        [HttpPost]
        [Route("getRating")]
        public async Task<IActionResult> getRatingAverage([FromBody] int idRecipe)
        {
            try
            {
                var avgRating = await _icalificacionService.GetRatingAverageByRecipe(idRecipe);
                return Ok(avgRating);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Ocurrió un error", error = ex.Message });
            }
        }

        [HttpPost]
        [Route("ratingByRecipe")]
        public async Task<IActionResult> getRatingsByRecipe([FromBody] int idRecipe)
        {
            try
            {
                var calificaciones = await _icalificacionService.GetRatingsByrecipeId(idRecipe);
                return Ok(calificaciones);
            }
            catch(Exception ex)
            {
                return BadRequest(new { message = "Ocurrió un error", error = ex.Message });
            }
        }
    }
}
