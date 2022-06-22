using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace IdentityApp.Pages.Identity.Admin
{
    public class RolesModel : AdminPageModel
    {
        public UserManager<IdentityUser> UserManager { get; }
        public RoleManager<IdentityRole> RoleManager { get; }

        [BindProperty(SupportsGet = true)]
        public string Id { get; set; }

        public IList<string> CurrentRoles { get; set; } = new List<string>();
        public IList<string> AvailableRoles { get; set; } = new List<string>();

        public string DashboardRole { get; }
        
        public RolesModel(UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IConfiguration config)
        {
            UserManager = userManager;
            RoleManager = roleManager;
            DashboardRole = config["Dashboard:Role"] ?? "Dashboard";
        }

        private async Task SetProperties()
        {
            IdentityUser user = await UserManager.FindByIdAsync(Id);
            CurrentRoles = await UserManager.GetRolesAsync(user);
            AvailableRoles = RoleManager.Roles.Select(r => r.Name)
                .Where(r => !CurrentRoles.Contains(r))
                .ToList();
        }

        public async Task<IActionResult> OnGetAsync()
        {
            if (string.IsNullOrEmpty(Id))
            {
                return RedirectToPage("SelectUser", new { Label = "Edit Roles", Callback = "roles" });
            }
            await SetProperties();
            return Page();
        }

        public async Task<IActionResult> OnPostAddToList(string role)
        {
            IdentityResult result = await RoleManager.CreateAsync(new IdentityRole(role));
            if (result.Process(ModelState))
            {
                return RedirectToPage();
            }
            await SetProperties();
            return Page();
        }
        
        public async Task<IActionResult> OnPostDeleteFromList(string role)
        {
            IdentityRole idRole = await RoleManager.FindByNameAsync(role);
            IdentityResult result = await RoleManager.DeleteAsync(idRole);
            if(result.Process(ModelState))
            {
                return RedirectToPage();
            }

            await SetProperties();
            return Page();
        }

        public async Task<IActionResult> OnPostAdd([Required] string role)
        {
            if(ModelState.IsValid)
            {
                IdentityResult result = IdentityResult.Success;
                if(result.Process(ModelState))
                {
                    IdentityUser user = await UserManager.FindByIdAsync(Id);
                    if(!await UserManager.IsInRoleAsync(user,role))
                    {
                        result = await UserManager.AddToRoleAsync(user, role);
                    }
                    if(result.Process(ModelState))
                    {
                        return RedirectToPage();
                    }
                }
            }
            await SetProperties();
            return Page();
        }
        
        public async Task<IActionResult> OnPostDelete(string role)
        {
            IdentityUser user = await UserManager.FindByIdAsync(Id);
            if(await UserManager.IsInRoleAsync(user, role))
            {
                await UserManager.RemoveFromRoleAsync(user, role);
            }
            return RedirectToPage();
        }
    }
}