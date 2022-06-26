using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace IdentityApp.Pages.Identity
{
    public class UserTwoFactorSetupModel : UserPageModel
    {
        public IdentityUser IdentityUser { get; set; }
        public string AuthenticatorKey { get; set; }
        public string QrCodeUrl { get; set; }

        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public UserTwoFactorSetupModel(UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            await LoadAuthenticatorKeys();

            if(await _userManager.GetTwoFactorEnabledAsync(IdentityUser))
            {
                return RedirectToPage("UserTwoFactorManage");
            }
            return Page();
        }

        public async Task<IActionResult> OnPostConfirm([Required] string confirm)
        {
            await LoadAuthenticatorKeys();
            if(ModelState.IsValid)
            {
                string token = Regex.Replace(confirm, @"\s", "");
                bool codeValid = await _userManager.VerifyTwoFactorTokenAsync(IdentityUser,
                    _userManager.Options.Tokens.AuthenticatorTokenProvider, token);
                
                if(codeValid)
                {
                    TempData["RecoveryCodes"] = await _userManager
                        .GenerateNewTwoFactorRecoveryCodesAsync(IdentityUser, 10);
                    await _userManager.SetTwoFactorEnabledAsync(IdentityUser, true);
                    await _signInManager.RefreshSignInAsync(IdentityUser);
                    return RedirectToPage("UserRecoveryCodes");
                } 
                else
                {
                ModelState.AddModelError(string.Empty,
                    "Confirmation code invalid");
                }
            }
            return Page();
        }

        private async Task LoadAuthenticatorKeys()
        {
            IdentityUser = await _userManager.GetUserAsync(User);
            AuthenticatorKey = await _userManager.GetAuthenticatorKeyAsync(IdentityUser);
            if(AuthenticatorKey == null)
            {
                await _userManager.ResetAuthenticatorKeyAsync(IdentityUser);
                AuthenticatorKey = await _userManager.GetAuthenticatorKeyAsync(IdentityUser);
                await _signInManager.RefreshSignInAsync(IdentityUser);
            }

            QrCodeUrl = $"otpauth://totp/ExampleApp:{IdentityUser.Email}"
            + $"?secret={AuthenticatorKey}";
        }
    }
}
