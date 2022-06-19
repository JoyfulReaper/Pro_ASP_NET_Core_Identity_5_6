using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IdentityApp.Pages.Identity.Admin
{
    public class DeleteModel : AdminPageModel
    {
        public IdentityUser IdentityUser { get; set; }

        [BindProperty(SupportsGet = true)]
        public string Id { get; set; }

        private readonly UserManager<IdentityUser> _userManager;

        public DeleteModel(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            if (string.IsNullOrEmpty(Id))
            {
                return RedirectToPage("Selectuser",
                new { Label = "Delete", Callback = "Delete" });
            }
            IdentityUser = await _userManager.FindByIdAsync(Id);
            return Page();
        }
        public async Task<IActionResult> OnPostAsync()
        {
            IdentityUser = await _userManager.FindByIdAsync(Id);
            IdentityResult result = await _userManager.DeleteAsync(IdentityUser);
            if (result.Process(ModelState))
            {
                return RedirectToPage("Dashboard");
            }
            return Page();
        }
    }
}
