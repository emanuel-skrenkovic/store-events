using System.Data;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Store.Shopping.Infrastructure;
using Store.Shopping.Infrastructure.Entity;

namespace Store.Shopping.Application.Products;

public class ProductInfoService
{
    private readonly IDbConnection _db;
    
    public ProductInfoService(StoreShoppingDbContext context)
        => _db = context?.Database.GetDbConnection() ?? throw new ArgumentNullException(nameof(context));
    
    public Task<IEnumerable<ProductInfo>> GetProductsInfoAsync(params string[] catalogueNumbers)
    {
        string query = 
            @"SELECT p.catalogue_number AS CatalogueNumber,
                     p.name,
                     p.price,
                     p.available
              FROM public.product p
              WHERE p.catalogue_number = ANY(@catalogueNumbers);";

        return _db.QueryAsync<ProductInfo>(query, new { catalogueNumbers });
    }
}