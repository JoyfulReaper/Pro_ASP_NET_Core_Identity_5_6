using IdentityApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IdentityApp.Pages;

[Authorize(Roles = "Admin")]
public class EditModel : PageModel
{
    public Product Product { get; set; }

    private readonly ProductDbContext _context;

    public EditModel(ProductDbContext context)
    {
        _context = context;
    }

    public void OnGet(long id)
    {
        Product = _context.Find<Product>(id) ?? new Product();
    }

    public IActionResult OnPost([Bind(Prefix="Product")] Product p)
    {
        _context.Update(p);
        _context.SaveChanges();
        return RedirectToPage("Admin");
    }
}
