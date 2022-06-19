using IdentityApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IdentityApp.Pages.Identity
{
    [AllowAnonymous]
    public class SignUpConfirmModel : UserPageModel
    {
        [BindProperty(SupportsGet = true)]
        public string Email { get; set; }
        
        [BindProperty(SupportsGet = true)]
        public string Token { get; set; }
        
        public bool ShowConfirmedMessage { get; set; } = false;

        private readonly UserManager<IdentityUser> _userManager;
        private readonly TokenUrlEncoderService _encoderService;

        public SignUpConfirmModel(UserManager<IdentityUser> userManager,
            TokenUrlEncoderService encoderService)
        {
            _userManager = userManager;
            _encoderService = encoderService;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            if (!string.IsNullOrEmpty(Email) && !string.IsNullOrEmpty(Token))
            {
                IdentityUser user = await _userManager.FindByEmailAsync(Email);
                if(user != null)
                {
                    string decodedToken = _encoderService.DecodeToken(Token);
                    IdentityResult result = await _userManager.ConfirmEmailAsync(user, decodedToken);
                    if(result.Process(ModelState))
                    {
                        ShowConfirmedMessage = true;
                    }
                }
            }
            return Page();
        }
    }
}
