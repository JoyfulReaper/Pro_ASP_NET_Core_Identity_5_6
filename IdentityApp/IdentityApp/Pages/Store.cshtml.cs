using IdentityApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IdentityApp.Pages;

[Authorize]
public class StoreModel : PageModel
{
    public ProductDbContext DbContext { get; set; }

    public StoreModel(ProductDbContext context)
    {
        DbContext = context;
    }
    
    public void OnGet()
    {
    }
}
