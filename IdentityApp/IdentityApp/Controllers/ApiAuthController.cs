using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace IdentityApp.Controllers;

[Route("api/auth")]
[ApiController]
public class ApiAuthController : ControllerBase
{
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IConfiguration _config;

    public ApiAuthController(SignInManager<IdentityUser> signInManager,
        UserManager<IdentityUser> userManager,
        IConfiguration config)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _config = config;
    }

    [HttpPost("signin")]
    public async Task<object> ApiSignIn([FromBody] SignInCredentials creds)
    {
        IdentityUser user = await _userManager.FindByEmailAsync(creds.Email);
        SignInResult result = await _signInManager.CheckPasswordSignInAsync(user, creds.Password, true);
        if (result.Succeeded)
        {
            SecurityTokenDescriptor descriptor = new SecurityTokenDescriptor
            {
                Subject = (await _signInManager.CreateUserPrincipalAsync(user))
                    .Identities.First(),
                Expires = DateTime.UtcNow.AddMinutes(
                    int.Parse(_config["BearerTokens:ExpiryMins"])),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["BearerTokens:Key"])),
                    SecurityAlgorithms.HmacSha256Signature)
            };
            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            SecurityToken secToken = handler
                .CreateToken(descriptor);
            
            return new { success = true, token = handler.WriteToken(secToken) };
        }

        return new { success = false };
    }
}