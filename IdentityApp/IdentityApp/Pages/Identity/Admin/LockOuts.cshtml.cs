using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IdentityApp.Pages.Identity.Admin
{
    public class LockOutsModel : AdminPageModel
    {
        public IEnumerable<IdentityUser> LockedOutUsers { get; set; }
        public IEnumerable<IdentityUser> OtherUsers { get; set; }

        private readonly UserManager<IdentityUser> _userManager;

        public LockOutsModel(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<TimeSpan> TimeLeft(IdentityUser user)
            => (await _userManager.GetLockoutEndDateAsync(user))
                .GetValueOrDefault()
                .Subtract(DateTimeOffset.Now);

        public void OnGet()
        {
            LockedOutUsers = _userManager.Users.Where(user => user.LockoutEnd.HasValue &&
                user.LockoutEnd.Value > DateTimeOffset.Now)
                .OrderBy(user => user.Email)
                .ToList();
            OtherUsers = _userManager.Users.Where(user => !user.LockoutEnd.HasValue ||
                user.LockoutEnd.Value <= DateTimeOffset.Now)
                .OrderBy(user => user.Email)
                .ToList();
        }

        public async Task<IActionResult> OnPostLockAsync(string id)
        {
            IdentityUser user = await _userManager.FindByIdAsync(id);
            await _userManager.SetLockoutEnabledAsync(user, true);
            await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.Now.AddDays(5));
            await _userManager.UpdateSecurityStampAsync(user);
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostUnlockAsync(string id)
        {
            IdentityUser user = await _userManager.FindByIdAsync(id);
            await _userManager.SetLockoutEndDateAsync(user, null);
            return RedirectToPage();
        }
    }
}
