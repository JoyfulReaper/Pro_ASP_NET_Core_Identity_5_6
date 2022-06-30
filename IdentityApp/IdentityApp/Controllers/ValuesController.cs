using IdentityApp.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace IdentityApp.Controllers;
[Route("api/data")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[ApiController]
public class ValuesController : ControllerBase
{
    private readonly ProductDbContext _dbContext;

    public ValuesController(ProductDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet]
    public IEnumerable<Product> GetProducts() =>
        _dbContext.Products;

    [HttpPost]
    public async Task<IActionResult> CreateProduct([FromBody] ProductBindingTarget targert)
    {
        Product product = new Product
        {
            Name = targert.Name,
            Price = targert.Price,
            Category = targert.Category
        };
        await _dbContext.AddAsync(product);
        await _dbContext.SaveChangesAsync();
        return Ok(product);
    }
    
    [HttpDelete("{id}")]
    public Task DeleteProduct(long id)
    {
        _dbContext.Products.Remove(new Product { Id = id });
        return _dbContext.SaveChangesAsync();
    }


    public class ProductBindingTarget
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public decimal Price { get; set; }

        [Required]
        public string Category { get; set; }
    }
}
