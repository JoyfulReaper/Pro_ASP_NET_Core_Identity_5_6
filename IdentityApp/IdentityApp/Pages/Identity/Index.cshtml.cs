using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IdentityApp.Pages.Identity
{
    public class IndexModel : UserPageModel
    {
        private readonly UserManager<IdentityUser> _userManager;

        public string Email { get; set; }
        public string Phone { get; set; }

        public IndexModel(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        public async void OnGet()
        {
            IdentityUser currentUser = await _userManager.GetUserAsync(User);
            Email = currentUser?.Email ?? "(No Value)";
            Phone = currentUser?.PhoneNumber ?? "(No Value)";
        }
    }
}
