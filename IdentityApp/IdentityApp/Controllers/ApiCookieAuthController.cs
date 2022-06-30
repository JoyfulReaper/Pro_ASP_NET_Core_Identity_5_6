using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace IdentityApp.Controllers;
[Route("api/cookieauth")]
[ApiController]
public class ApiCookieAuthController : ControllerBase
{
    private readonly SignInManager<IdentityUser> _signInManager;

    public ApiCookieAuthController(SignInManager<IdentityUser> signInManager)
    {
        _signInManager = signInManager;
    }

    [HttpPost("signin")]
    public async Task<object> ApiSignIn([FromBody] SignInCredentials creds)
    {
        SignInResult result = await _signInManager.PasswordSignInAsync(
            creds.Email, creds.Password, true, true);
        return new { success = result.Succeeded };
    }

    [HttpPost("signout")]
    public async Task<IActionResult> ApiSignOut()
    {
        await _signInManager.SignOutAsync();
        return Ok();
    }
}

public class SignInCredentials
{
    [Required]
    public string Email { get; set; }

    [Required]
    public string Password { get; set; }
}