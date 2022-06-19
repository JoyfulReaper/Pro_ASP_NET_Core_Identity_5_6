using IdentityApp.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace IdentityApp.Pages.Identity
{
    public class SignUpModel : PageModel
    {
        [BindProperty]
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [BindProperty]
        [Required]
        public string Password { get; set; }

        private readonly UserManager<IdentityUser> _userManager;
        private readonly IdentityEmailService _emailService;

        public SignUpModel(UserManager<IdentityUser> userManager,
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
                if (user != null && !await _userManager.IsEmailConfirmedAsync(user))
                {
                    return RedirectToPage("SignUpConfirm");
                }
                user = new IdentityUser
                {
                    UserName = Email,
                    Email = Email
                };

                IdentityResult result = await _userManager.CreateAsync(user);
                if (result.Process(ModelState))
                {
                    result = await _userManager.AddPasswordAsync(user, Password);
                    if (result.Process(ModelState))
                    {
                        await _emailService.SendAccountConfirmationEmail(user,
                            "SignUpConfirm");
                    }
                    return RedirectToPage("SignUpConfirm");
                }
                else
                {
                    await _userManager.DeleteAsync(user);
                }
            }
            return Page();
        }
    }
}
