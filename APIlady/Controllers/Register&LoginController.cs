using APIlady.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace APIlady.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Register_LoginController : ControllerBase
    {
        UserManager<ApplicationUser> _userManager;
        private string GlobalToken;

        public Register_LoginController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register(RegisterDto UserFromUrl)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser user = new ApplicationUser();
                user.UserName = UserFromUrl.Name;
                user.Email = UserFromUrl.Email;
                var result = await _userManager.CreateAsync(user, UserFromUrl.Password);
                if (result.Succeeded) return Ok("Created");
                else foreach (var e in result.Errors)
                    {
                        ModelState.AddModelError("Errorr", e.Description);
                    }
            }
            return BadRequest(ModelState);
        }

        [HttpPost("Login")]
        public async  Task<IActionResult> Login(LoginDto UserFromUrl)
        {
            if (ModelState.IsValid)
            {
                var User = await _userManager.FindByNameAsync(UserFromUrl.UserName);
                if (User != null)
                {
                    var result = await _userManager.CheckPasswordAsync(User, UserFromUrl.Password);
                    if (result)
                    {

                        //claims
                        var UserClaims = new List<Claim>();
                        UserClaims.Add(new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()));
                        UserClaims.Add(new Claim(ClaimTypes.NameIdentifier,User.Id.ToString()));
                        UserClaims.Add(new Claim(ClaimTypes.Name, User.UserName));
                        var userRoles = await _userManager.GetRolesAsync(User);
                        foreach (var role in userRoles)
                        {
                            UserClaims.Add(new Claim(ClaimTypes.Role, role.ToString()));
                        }


                        //SignIn key
                        SymmetricSecurityKey SignInKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes("asldfjbaskjfvb@#$4584545kasjfbfvdfslkasfl"));

                        //Credentials
                        SigningCredentials signingCredentials = new SigningCredentials(
                           SignInKey, SecurityAlgorithms.HmacSha256);


                        //Design token
                        JwtSecurityToken Mytoken = new JwtSecurityToken(
                            audience: "mohamed",
                            issuer: "aboellil",
                            expires: DateTime.Now.AddDays(1),
                            claims: UserClaims,
                            signingCredentials: signingCredentials
                            );


                        //RefreshToken
                        string ActiveRefreshToken;
                        DateTime ActiveRefreshTokenExpiration;
                        if (User.RefreshTokens.Any(t => t.IsActive))
                        {
                            ActiveRefreshToken = User.RefreshTokens.FirstOrDefault(t => t.IsActive).Token;
                            ActiveRefreshTokenExpiration = User.RefreshTokens.FirstOrDefault(t => t.IsActive).ExpiresOn;
                        }
                        else
                        {
                            ActiveRefreshToken = GenerateRefreshToken().Token;
                            ActiveRefreshTokenExpiration = GenerateRefreshToken().ExpiresOn;
                            User.RefreshTokens.Add(GenerateRefreshToken());
                            await _userManager.UpdateAsync(User);
                        }
                        SetRefreshTokenInCookies(ActiveRefreshToken, ActiveRefreshTokenExpiration);

                        //Genertae Token
                        return Ok(new
                        {
                            token = new JwtSecurityTokenHandler().WriteToken(Mytoken),
                            expiration = DateTime.Now.AddDays(1),
                        });


                    }
                }
                ModelState.AddModelError("UserName", "UaserName or Password Are Invalid");
            }
            return BadRequest(ModelState);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public RefreshToken GenerateRefreshToken()
        {
            var randomNum = new byte[32];
            using var generator = new RNGCryptoServiceProvider(randomNum);
            generator.GetBytes(randomNum);
            return new RefreshToken
            {
                Token = Convert.ToBase64String(randomNum),
                ExpiresOn= DateTime.Now.AddDays(10),
                CreatedOn = DateTime.Now,
            };
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public void SetRefreshTokenInCookies(string refreshToken,DateTime ex)
        {
            var cookieOptions = new CookieOptions()
            {
                HttpOnly = true,
                Expires = ex.ToLocalTime(),

            };
            Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
        }
    }
}
