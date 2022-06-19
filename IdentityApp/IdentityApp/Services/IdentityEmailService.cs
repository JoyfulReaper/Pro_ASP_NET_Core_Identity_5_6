using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace IdentityApp.Services;

public class IdentityEmailService
{
    private readonly IEmailSender _emailSender;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IHttpContextAccessor _contextAccessor;
    private readonly LinkGenerator _linkGenerator;
    private readonly TokenUrlEncoderService _encoder;

    public IdentityEmailService(IEmailSender emailSender,
        UserManager<IdentityUser> userManager,
        IHttpContextAccessor contextAccessor,
        LinkGenerator linkGenerator,
        TokenUrlEncoderService encoder)
    {
        _emailSender = emailSender;
        _userManager = userManager;
        _contextAccessor = contextAccessor;
        _linkGenerator = linkGenerator;
        _encoder = encoder;
    }

    public async Task SendPasswordRecoveryEmail(IdentityUser user,
        string confirmationPage)
    {
        string token = await _userManager.GeneratePasswordResetTokenAsync(user);
        string url = GetUrl(user.Email, token, confirmationPage);
        await _emailSender.SendEmailAsync(user.Email, "Set Your Password",
            $"Please set your password by clicking <a href='{url}'>here</a>.");
    }

    public async Task SendAccountConfirmationEmail(IdentityUser user, string confirmationPage)
    {
        string token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        string url = GetUrl(user.Email, token, confirmationPage);
        await _emailSender.SendEmailAsync(user.Email, "Complete Your Account Setup",
            $"Please confirm your account by clicking <a href='{url}'>here</a>.");
    }

    private string GetUrl(string emailAddress, string token, string page)
    {
        string safeToken = _encoder.EncodeToken(token);
        return _linkGenerator.GetUriByPage(_contextAccessor.HttpContext, page,
            null, new { email = emailAddress, token = safeToken });
    }
}
