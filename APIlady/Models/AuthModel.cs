using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace APIlady.Models
{
    public class AuthModel
    {
        public int Id { get; set; }
        public string? Messege { get; set; }
        public string? Token { get; set; }
        public string? Username { get; set; }
        public string? Email { get; set; }
        public List<string>? Roles { get; set; }
        [JsonIgnore]
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiration { get; set; }
    }
}
