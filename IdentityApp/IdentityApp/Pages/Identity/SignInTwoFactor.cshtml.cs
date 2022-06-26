using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace IdentityApp.Pages.Identity
{
    [AllowAnonymous]
    public class SignInTwoFactorModel : UserPageModel
    {
        [BindProperty]
        public string? ReturnUrl { get; set; }
        
        [BindProperty]
        [Required]
        public string Token { get; set; }
        
        [BindProperty]
        public bool RememberMe { get; set; }

        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public SignInTwoFactorModel(UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }
        
        public async Task<IActionResult> OnPostAsync()
        {
            if(ModelState.IsValid)
            {
                IdentityUser user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
                if(user != null)
                {
                    string token = Regex.Replace(Token, @"\s", "");
                    Microsoft.AspNetCore.Identity.SignInResult result = await
                        _signInManager.TwoFactorAuthenticatorSignInAsync(token, true, RememberMe);
                    if(!result.Succeeded)
                    {
                        result = await _signInManager.TwoFactorRecoveryCodeSignInAsync(token);
                    }
                    if(result.Succeeded)
                    {
                        if(await _userManager.CountRecoveryCodesAsync(user) <= 3)
                        {
                            return RedirectToPage("SignInCodesWarning");
                        }
                        return Redirect(ReturnUrl ?? "/");
                    }
                }
                ModelState.AddModelError(string.Empty, "Invalid token or recovery code.");
            }
            return Page();
        }
    }
}
