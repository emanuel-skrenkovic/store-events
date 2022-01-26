using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Store.Catalogue.AspNet.Commands;
using Store.Catalogue.AspNet.Models;
using Store.Catalogue.Infrastructure;
using Store.Catalogue.Infrastructure.Entity;
using Store.Core.Domain;
using Store.Core.Infrastructure.AspNet;

namespace Store.Catalogue.AspNet.Controllers;

[ApiController]
[Route("[controller]")]
public class ProductsController : ControllerBase
{
    private readonly StoreCatalogueDbContext _context;
    private readonly CursorHandler _cursorHandler;

    public ProductsController(StoreCatalogueDbContext context, CursorHandler cursorHandler)
    {
        _context = Ensure.NotNull(context);
        _cursorHandler = Ensure.NotNull(cursorHandler);
    }

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
            
        const string query = 
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

    [HttpGet]
    public async Task<IActionResult> GetProducts(
        [FromQuery] int limit = 10, //TODO: cleaner
        [FromQuery] string cursor = null)
    {
        IDbConnection db = _context.Database.GetDbConnection();

        const string query =
            @"SELECT * 
              FROM public.product
              WHERE id >= @cursor
              ORDER BY id
              LIMIT @limit;";

        var queryParameters = new
        {
            limit = limit + 1, 
            cursor = string.IsNullOrWhiteSpace(cursor) 
                ? 0 : 
                _cursorHandler.Parse<int>(cursor)
        };
                
        IEnumerable<ProductEntity> productEntities = 
            (await db.QueryAsync<ProductEntity>(query, queryParameters)).ToArray();

        ProductApiModel[] products = productEntities
            .Take(limit)
            .Select(e => new ProductApiModel
            {
                CatalogueId = e.CatalogueId,
                CreatedAt   = e.CreatedAt,
                UpdatedAt   = e.UpdatedAt,
                Name        = e.Name,
                Description = e.Description,
                Available   = e.Available
            }).ToArray();

        return Ok(new PagedResponse<ProductApiModel>(
                products, 
                _cursorHandler.Compose(productEntities.LastOrDefault()?.Id)));
    }
}