using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace IdentityApp.Pages.Identity
{
    [AllowAnonymous]
    public class SignUpExternalModel : UserPageModel
    {
        public IdentityUser IdentityUser { get; set; }

        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public SignUpExternalModel(UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<string> ExternalProvider() =>
            (await _userManager.GetLoginsAsync(IdentityUser))
            .FirstOrDefault()?.ProviderDisplayName;
        
        public IActionResult OnPost(string provider)
        {
            string callbackUrl = Url.Page("SignUpExternal", "Callback");
            AuthenticationProperties props =
            _signInManager.ConfigureExternalAuthenticationProperties(
            provider, callbackUrl);
            return new ChallengeResult(provider, props);
        }

        public async Task<IActionResult> OnGetCallbackAsync()
        {
            ExternalLoginInfo info = await _signInManager.GetExternalLoginInfoAsync();
            string email = info?.Principal?.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email))
            {
                return Error("External service has not provided an email address.");
            }
            else if ((await _userManager.FindByEmailAsync(email)) != null)
            {
                return Error("An account already exists with your email address.");
            }
            IdentityUser identUser = new IdentityUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true
            };
            IdentityResult result = await _userManager.CreateAsync(identUser);
            if (result.Succeeded)
            {
                identUser = await _userManager.FindByEmailAsync(email);
                result = await _userManager.AddLoginAsync(identUser, info);
                return RedirectToPage(new { id = identUser.Id });
            }
            return Error("An account could not be created.");
        }
        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null)
            {
                return RedirectToPage("SignUp");
            }
            else
            {
                IdentityUser = await _userManager.FindByIdAsync(id);
                if (IdentityUser == null)
                {
                    return RedirectToPage("SignUp");
                }
            }
            return Page();
        }
        private IActionResult Error(string err)
        {
            TempData["errorMessage"] = err;
            return RedirectToPage();
        }
    }
}

