using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace IdentityApp.Pages.Identity.Admin;

public class EditModel : AdminPageModel
{
    private readonly UserManager<IdentityUser> _userManager;

    public EditModel(UserManager<IdentityUser> userManager)
    {
        _userManager = userManager;
    }

    public IdentityUser IdentityUser { get; set; }

    [BindProperty(SupportsGet = true)]
    public string Id { get; set; }


    public async Task<IActionResult> OnGetAsync()
    {
        if (string.IsNullOrEmpty(Id))
        {
            return RedirectToPage("Selectuser",
            new { Label = "Edit User", Callback = "Edit" });
        }
        IdentityUser = await _userManager.FindByIdAsync(Id);
        return Page();
    }

    public async Task<IActionResult> OnPostAsync([FromForm(Name="IdentityUser")] EditBindingTarget userData)
    {
        if(!string.IsNullOrEmpty(Id) && ModelState.IsValid)
        {
            IdentityUser user = await _userManager.FindByIdAsync(Id);
            if(user != null)
            {
                user.UserName = userData.Email;
                user.Email = userData.Email;
                user.EmailConfirmed = true;
                if(!string.IsNullOrEmpty(userData.PhoneNumber))
                {
                    user.PhoneNumber = userData.PhoneNumber;
                }
            }
            IdentityResult result = await _userManager.UpdateAsync(user);
            if(result.Process(ModelState))
            {
                return RedirectToPage();
            }
        }
        IdentityUser = await _userManager.FindByIdAsync(Id);
        return Page();
    }
}

public class EditBindingTarget
{
    [Required]
    public string Username { get; set; }

    [Required]
    public string Email { get; set; }

    [Phone]
    public string? PhoneNumber { get; set; }
}
