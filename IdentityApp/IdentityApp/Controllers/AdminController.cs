using IdentityApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdentityApp.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly ProductDbContext _context;

    public AdminController(ProductDbContext context)
    {
        _context = context;
    }
    
    public IActionResult Index()
    {
        return View(_context.Products);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View("Edit", new Product());
    }

    [HttpGet]
    public IActionResult Edit(long id)
    {
        var product = _context.Products.Find(id);
        if( product != null)
        {
            return View("Edit", product);
        }
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public IActionResult Save(Product p)
    {
        _context.Update(p);
        _context.SaveChanges();
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public IActionResult Delete(long id)
    {
        Product p = _context.Find<Product>(id);
        if (p != null)
        {
            _context.Remove(p);
            _context.SaveChanges();
        }
        return RedirectToAction(nameof(Index));
    }
}
