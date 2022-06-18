using IdentityApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace IdentityApp.Pages.Identity
{
    [AllowAnonymous]
    public class UserPasswordRecoveryModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IdentityEmailService _emailService;

        public UserPasswordRecoveryModel(UserManager<IdentityUser> userManager,
            IdentityEmailService emailService)
        {
            _userManager = userManager;
            _emailService = emailService;
        }

        public async Task<IActionResult> OnPostAsync([Required]string email)
        {
            if (ModelState.IsValid)
            {
                IdentityUser user = await _userManager.FindByEmailAsync(email);
                if (user != null)
                {
                    await _emailService.SendPasswordRecoveryEmail(user, "UserPasswordRecoveryConfirm");
                    TempData["message"] = "We couldn't find that user";
                }
                TempData["message"] = "We have send you an email. " +
                    "Click the link it contains to choose a new password.";
                return RedirectToPage();
            }
            return Page();
        }
    }
}
