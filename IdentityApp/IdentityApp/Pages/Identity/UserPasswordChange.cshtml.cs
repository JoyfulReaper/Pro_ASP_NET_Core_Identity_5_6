using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace IdentityApp.Pages.Identity
{
    public class UserPasswordChangeModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;

        public UserPasswordChangeModel(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }
        
        public async Task<IActionResult> OnPostAsync(PasswordChangeBindingTarget data)
        {
            if (ModelState.IsValid)
            {
                IdentityUser user = await _userManager.GetUserAsync(User);
                IdentityResult result = await _userManager.ChangePasswordAsync(user, data.Current, data.NewPassword);
                
                if(result.Process(ModelState))
                {
                    TempData["message"] = "Password Changed";
                    return RedirectToPage();
                }
            }
            return Page();
        }
    }

    public class PasswordChangeBindingTarget
    {
        [Required]
        public string Current { get; set; }

        [Required]
        public string NewPassword { get; set; }

        [Required]
        [Compare("NewPassword")]
        public string ConfirmPassword { get; set; }
    }
}
