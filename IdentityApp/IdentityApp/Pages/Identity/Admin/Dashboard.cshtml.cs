using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IdentityApp.Pages.Identity.Admin
{
    public class DashboardModel : AdminPageModel
    {
        private readonly UserManager<IdentityUser> _userManager;

        public int UsersCount { get; set; } = 0;
        public int UsersUnconfirmed { get; set; } = 0;
        public int UsersLockedout { get; set; } = 0;
        public int UsersTwoFactor { get; set; } = 0;

        private readonly string[] emails = {
            "alice@example.com", "bob@example.com", "charlie@example.com"
        };

        public DashboardModel(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        public void OnGet()
        {
            UsersCount = _userManager.Users.Count();
            UsersUnconfirmed = _userManager.Users
                .Count(u => !u.EmailConfirmed);
            UsersLockedout = _userManager.Users
                .Where(u => u.LockoutEnabled && u.LockoutEnd > System.DateTimeOffset.Now)
                .Count();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // ToList() method to force evaluation in the foreach loop.
            // This ensures I don’t cause an error by deleting objects from the sequence that I am enumerating.
            foreach (IdentityUser existingUser in _userManager.Users.ToList())
            {
                IdentityResult result = await _userManager.DeleteAsync(existingUser);
                result.Process(ModelState);
            }
            
            foreach (string email in emails)
            {
                IdentityUser userObject = new IdentityUser
                {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true,
                };

                IdentityResult result = await _userManager.CreateAsync(userObject);
                if(result.Process(ModelState))
                {
                    result = await _userManager.AddPasswordAsync(userObject, "mysecret");
                    result.Process(ModelState);
                }
            }

            if (ModelState.IsValid)
            {
                return RedirectToPage();
            }
            return Page();
        }
    }
}
