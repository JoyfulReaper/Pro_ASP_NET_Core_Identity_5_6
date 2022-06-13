using IdentityApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdentityApp.Controllers;

[Authorize]
public class StoreController : Controller
{
    private readonly ProductDbContext _context;

    public StoreController(ProductDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        return View(_context.Products);
    }
}
