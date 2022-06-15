using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IdentityApp.Pages.Identity.Admin
{
    public class SelectUserModel : AdminPageModel
    {
        private readonly UserManager<IdentityUser> _userManager;

        public IEnumerable<IdentityUser> Users { get; set; }

        [BindProperty(SupportsGet = true)]
        public string Label { get; set; }

        [BindProperty(SupportsGet = true)]
        public string Callback { get; set; }

        [BindProperty(SupportsGet = true)]
        public string Filter { get; set; }

        public SelectUserModel(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        public void OnGet()
        {
            Users = _userManager.Users
                .Where(u => Filter == null || u.UserName.Contains(Filter))
                .OrderBy(u => u.Email)
                .ToList(); // ToList method to force the evaluation of the query so that I can
                           // operate on the results without triggering repeated user store searches.
        }

        public IActionResult OnPost()
        {
            return RedirectToPage(new { Filter, Callback });
        }
    }
}
