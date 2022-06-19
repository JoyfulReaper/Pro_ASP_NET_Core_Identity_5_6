using IdentityApp.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace IdentityApp.Pages.Identity.Admin
{
    public class CreateModel : AdminPageModel
    {
        [BindProperty(SupportsGet = true)]
        [EmailAddress]
        public string Email { get; set; }

        private readonly UserManager<IdentityUser> _userManager;
        private readonly IdentityEmailService _emailService;

        public CreateModel(UserManager<IdentityUser> userManager,
            IdentityEmailService emailService)
        {
            _userManager = userManager;
            _emailService = emailService;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                IdentityUser user = new IdentityUser
                {
                    UserName = Email,
                    Email = Email,
                    EmailConfirmed = true,
                };
                IdentityResult result = await _userManager.CreateAsync(user);
                if(result.Process(ModelState))
                {
                    await _emailService.SendPasswordRecoveryEmail(user,
                        "/Identity/UserAccountComplete");
                    TempData["message"] = "Account Created";
                    return RedirectToPage();
                }
            }
            return Page();
        }
    }
}
