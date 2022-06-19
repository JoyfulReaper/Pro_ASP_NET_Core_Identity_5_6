using IdentityApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IdentityApp.Pages.Identity
{
    [AllowAnonymous]
    public class SignUpResendModel : UserPageModel
    {
        [BindProperty(SupportsGet = true)]
        public string Email { get; set; }

        private readonly UserManager<IdentityUser> _userManager;
        private readonly IdentityEmailService _emailService;

        public SignUpResendModel(UserManager<IdentityUser> userManager,
            IdentityEmailService emailService)
        {
            _userManager = userManager;
            _emailService = emailService;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                IdentityUser user = await _userManager.FindByEmailAsync(Email);
                if(user != null && !await _userManager.IsEmailConfirmedAsync(user))
                {
                    await _emailService.SendAccountConfirmationEmail(user, "SignUpConfirm");
                }
                TempData["message"] = "Confirmation Email Sent. Check your inbox.";
                return RedirectToPage(new {Email});
            }
            return Page();
        }
    }
}
