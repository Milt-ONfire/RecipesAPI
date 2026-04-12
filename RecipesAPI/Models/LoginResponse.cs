namespace RecipesAPI.Models
{
    public class LoginResponse
    {
        public string? UserName { get; set; }
        public string? AccesToken { get; set; }
        public int ExpiresIn { get; set; }
    }
}
