using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace IdentityApp.Pages.Identity;

[AllowAnonymous]
public class SignInModel : UserPageModel
{
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly UserManager<IdentityUser> _userManager;

    [Required]
    [EmailAddress]
    [BindProperty]
    public string Email { get; set; }

    [Required]
    [BindProperty]
    public string Password { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? ReturnUrl { get; set; }

    public SignInModel(SignInManager<IdentityUser> signInManager,
        UserManager<IdentityUser> userManager)
    {
        _signInManager = signInManager;
        _userManager = userManager;
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (ModelState.IsValid)
        {
            SignInResult result = await _signInManager.PasswordSignInAsync(Email,
                Password, true, true);

            if (result.Succeeded)
            {
                return Redirect(ReturnUrl ?? "/");
            }
            else if (result.IsLockedOut)
            {
                TempData["message"] = "Account Locked";
            }
            else if (result.IsNotAllowed)
            {
                IdentityUser user = await _userManager.FindByEmailAsync(Email);
                if (user != null && !await _userManager.IsEmailConfirmedAsync(user))
                {
                    return RedirectToPage("SignUpConfirm");
                }
                TempData["message"] = "Sign In Not Allowed";
            }
            else if (result.RequiresTwoFactor)
            {
                return RedirectToPage("SignInTwoFactor", new { ReturnUrl });
            }
            else
            {
                TempData["message"] = "Sign In Failed";
            }
        }
        return Page();
    }
}
