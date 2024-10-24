using Microsoft.AspNetCore.Identity;

namespace APIlady.Models
{
    public class ApplicationUser : IdentityUser
    {
        public List<RefreshToken>? RefreshTokens { get; set; }
    }
}
