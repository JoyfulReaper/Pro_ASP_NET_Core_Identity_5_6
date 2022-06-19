using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IdentityApp.Pages.Identity
{
    public class UserDeleteModel : UserPageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public UserDeleteModel(UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            IdentityUser idUser = await _userManager.GetUserAsync(User);
            IdentityResult result = await _userManager.DeleteAsync(idUser);
            if (result.Process(ModelState))
            {
                await _signInManager.SignOutAsync();
                return Challenge();
            }
            return Page();
        }
    }
}
