using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RecipesAPI.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace RecipesAPI.Services
{
    public class JwtService
    {
        private readonly RecipesDbContext _dbContext;
        private readonly IConfiguration _configuration;
        public JwtService(RecipesDbContext dbContext, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _configuration = configuration;
        }

        public async Task<LoginResponse?> Authenticate(LoginRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.UserName) || string.IsNullOrWhiteSpace(request.Password))
                return null;

            var userAccount = await _dbContext.Usuarios.FirstOrDefaultAsync(x => x.NombreUsuario == request.UserName);

            var passwordHasher = new PasswordHasher<Usuario>();

            if (userAccount is null)
                return null;

            var verification = passwordHasher.VerifyHashedPassword(
                userAccount,
                userAccount.Password,        // contraseña almacenada (HASH)
                request.Password               // contraseña que el usuario envía
            );

            if (verification == PasswordVerificationResult.Failed)
                return null;

            var issuer = _configuration["Jwt:Issuer"];
            var audience = _configuration["Jwt:Audience"];
            var key = _configuration["Jwt:Key"];
            var tokenValidityMins = _configuration.GetValue<int>("Jwt:TokenValidityMins");
            var tokenExpiryTimeStamp = DateTime.UtcNow.AddMinutes(tokenValidityMins);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new System.Security.Claims.ClaimsIdentity(new[]
                {
                    new System.Security.Claims.Claim(JwtRegisteredClaimNames.Name, request.UserName),
                    new System.Security.Claims.Claim(JwtRegisteredClaimNames.NameId, userAccount.IdUsuario.ToString()),
                    new System.Security.Claims.Claim(JwtRegisteredClaimNames.Profile,userAccount.Imagen != null ? userAccount.Imagen.ToString() : ""),
                    new System.Security.Claims.Claim(JwtRegisteredClaimNames.Email, userAccount.Email.ToString())

                }),
                Issuer = issuer,
                Expires = tokenExpiryTimeStamp,
                Audience = audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key)),
                    SecurityAlgorithms.HmacSha512Signature),
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.CreateToken(tokenDescriptor);
            var accesToken = tokenHandler.WriteToken(securityToken);

            return new LoginResponse
            {
                AccesToken = accesToken,
                UserName = request.UserName,
                ExpiresIn = (int)tokenExpiryTimeStamp.Subtract(DateTime.UtcNow).TotalSeconds
            };
        }
    }
}
