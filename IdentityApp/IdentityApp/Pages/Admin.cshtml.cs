using IdentityApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IdentityApp.Pages;

[Authorize(Roles = "Admin")]
public class AdminModel : PageModel
{
    public ProductDbContext DbContext { get; }

    public AdminModel(ProductDbContext context)
    {
        DbContext = context;
    }

    public IActionResult OnPost(long id)
    {
        Product p = DbContext.Find<Product>(id);
        if(p != null)
        {
            DbContext.Remove(p);
            DbContext.SaveChanges();
        }
        return Page();
    }
}
