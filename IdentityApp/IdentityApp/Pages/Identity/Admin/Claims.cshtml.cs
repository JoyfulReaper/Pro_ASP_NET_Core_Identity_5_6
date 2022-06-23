using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace IdentityApp.Pages.Identity.Admin
{
    public class ClaimsModel : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public string Id { get; set; }

        public IEnumerable<Claim> Claims { get; set; }

        private readonly UserManager<IdentityUser> _userManager;

        public ClaimsModel(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            if (string.IsNullOrEmpty(Id))
            {
                return RedirectToPage("Selectuser",
                new { Label = "Manage Claims", Callback = "Claims" });
            }
            IdentityUser user = await _userManager.FindByIdAsync(Id);
            Claims = await _userManager.GetClaimsAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync([Required] string task,
            [Required] string type, [Required] string value, string? oldValue)
        {
            IdentityUser user = await _userManager.FindByIdAsync(Id);
            Claims = await _userManager.GetClaimsAsync(user);
            if(ModelState.IsValid)
            {
                Claim claim = new Claim(type, value);
                IdentityResult result = IdentityResult.Success;
                switch (task)
                {
                    case "add":
                        result = await _userManager.AddClaimAsync(user, claim);
                        break;
                    case "change":
                        result = await _userManager.ReplaceClaimAsync(user,
                            new Claim(type, oldValue), claim);
                        break;
                    case "delete":
                        result = await _userManager.RemoveClaimAsync(user, claim);
                        break;
                    default:
                        break;
                };
                if (result.Process(ModelState))
                {
                    return RedirectToPage();
                }
            }
            return Page();
        }
    }
}
