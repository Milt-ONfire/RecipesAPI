using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecipesAPI.Models;
using RecipesAPI.Services;

namespace RecipesAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserAccountController : ControllerBase
    {
        private readonly JwtService _jwtService;

        public UserAccountController(JwtService jwtService) =>

            _jwtService = jwtService;


        [AllowAnonymous]
        [HttpPost("Login")]

        public async Task<ActionResult<object>> Login(LoginRequest request)
        {
            try
            {
                var result = await _jwtService.Authenticate(request);
                if (result == null)
                {
                    return Unauthorized();
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = "Ocurrió un error", message = ex.Message });
            }




        }
    }
}
