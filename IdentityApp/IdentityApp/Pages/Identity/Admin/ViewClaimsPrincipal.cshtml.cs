using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace IdentityApp.Pages.Identity.Admin
{
    public class ViewClaimsPrincipalModel : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public string Id { get; set; }

        [BindProperty(SupportsGet = true)]
        public string Callback { get; set; }

        public ClaimsPrincipal Principal { get; set; }
       

        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public ViewClaimsPrincipalModel(UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            if (string.IsNullOrEmpty(Id))
            {
                return RedirectToPage("Selectuser",
                    new { Label = "View ClaimsPrincipal", Callback = "ClaimsPrincipal" });
            }
            IdentityUser user = await _userManager.FindByIdAsync(Id);
            Principal = await _signInManager.CreateUserPrincipalAsync(user);
            return Page();
        }
    }
}
