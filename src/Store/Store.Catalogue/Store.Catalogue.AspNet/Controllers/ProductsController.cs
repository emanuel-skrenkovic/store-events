using System;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Store.Catalogue.AspNet.Commands;
using Store.Catalogue.AspNet.Models;
using Store.Catalogue.Infrastructure;
using Store.Catalogue.Infrastructure.Entity;

namespace Store.Catalogue.AspNet.Controllers;

[ApiController]
[Route("[controller]")]
public class ProductsController : ControllerBase
{
    private readonly StoreCatalogueDbContext _context;

    public ProductsController(StoreCatalogueDbContext context)
        => _context = context ?? throw new ArgumentNullException(nameof(context));

    [HttpPost]
    public async Task<IActionResult> CreateProduct([FromBody] ProductCreateCommand command)
    {
        Guid productCatalogueId = Guid.NewGuid();
            
        ProductApiModel productModel = command.Product;

        DateTime now = DateTime.UtcNow;
        _context.Products.Add(new()
        {
            CatalogueId = productCatalogueId,
            UpdatedAt = now,
            CreatedAt = now,
            Name = productModel.Name,
            Price = productModel.Price,
            Available = productModel.Available,
            Description = productModel.Description
        });

        await _context.SaveChangesAsync();
            
        return CreatedAtAction("GetProduct", new { id = productCatalogueId }, null);
    }

    [HttpPut]
    [Route("{id:guid}")]
    public async Task<IActionResult> UpdateProduct(
        [FromRoute] Guid id,
        [FromBody] ProductUpdateCommand command)
    {
        DbSet<ProductEntity> set = _context.Products;
        
        ProductEntity productEntity = await set.SingleOrDefaultAsync(p => p.CatalogueId == id);
        if (productEntity == null) return NotFound();
        
        ProductApiModel productModel = command.Product;

        productEntity.UpdatedAt   = DateTime.UtcNow;
        productEntity.Name        = productModel.Name;
        productEntity.Price       = productModel.Price;
        productEntity.Available   = productModel.Available;
        productEntity.Description = productModel.Description;

        set.Update(productEntity);

        await _context.SaveChangesAsync();

        return Ok();
    }

    [HttpGet]
    [Route("{id:guid}")]
    public async Task<IActionResult> GetProduct([FromRoute] Guid id)
    {
        IDbConnection db = _context.Database.GetDbConnection();
            
        string query = 
            @"SELECT p.catalogue_id as CatalogueId,
                     p.created_at as CreatedAt,
                     p.updated_at as UpdatedAt,
                     p.name,
                     p.price,
                     p.available,
                     p.description
                  FROM public.product p
                  WHERE p.catalogue_id = @id;";

        ProductApiModel product = await db.QueryFirstOrDefaultAsync<ProductApiModel>(query, new { id });
        if (product == null) return NotFound();

        return Ok(product);
    }
}