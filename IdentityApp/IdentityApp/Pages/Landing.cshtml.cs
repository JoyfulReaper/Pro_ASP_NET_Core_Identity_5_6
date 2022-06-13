using IdentityApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IdentityApp.Pages;

public class LandingModel : PageModel
{
    public ProductDbContext DbContext;

    public LandingModel(ProductDbContext context)
    {
        DbContext = context;
    }
    
    public void OnGet()
    {
    }
}
