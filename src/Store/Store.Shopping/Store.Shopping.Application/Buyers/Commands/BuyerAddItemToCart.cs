using System.Data;
using Dapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Store.Core.Domain;
using Store.Core.Domain.ErrorHandling;
using Store.Shopping.Domain;
using Store.Shopping.Domain.Buyers;
using Store.Shopping.Domain.Buyers.ValueObjects;
using Store.Shopping.Infrastructure;
using Store.Shopping.Infrastructure.Entity;

namespace Store.Shopping.Application.Buyers.Commands;

public record BuyerAddItemToCartCommand
(
    string CustomerNumber, 
    string SessionId, 
    string ProductCatalogueNumber
) : IRequest<Result>;

public class BuyerAddItemToCartCommandHandler : IRequestHandler<BuyerAddItemToCartCommand, Result>
{
    private readonly IAggregateRepository _repository;
    private readonly IDbConnection _db;

    public BuyerAddItemToCartCommandHandler(IAggregateRepository repository, StoreShoppingDbContext context)
    {
        _repository = Ensure.NotNull(repository);
        _db         = Ensure.NotNull(context?.Database.GetDbConnection());
    }
        
    public async Task<Result> Handle(BuyerAddItemToCartCommand request, CancellationToken ct)
    {
        (string customerNumber, string sessionId, string productCatalogueNumber) = request;

        const string query =
            @"SELECT 
                  p.*
              FROM 
                  public.product p
              WHERE 
                  p.catalogue_number = @productCatalogueNumber;";

        ProductEntity product = await _db.QuerySingleOrDefaultAsync<ProductEntity>
        (
            query, 
            new { productCatalogueNumber }
        );

        if (product == null)    return new Error($"Could not find product with catalogue number: '{productCatalogueNumber}'.");
        if (!product.Available) return new Error($"Product with catalogue number '{productCatalogueNumber}' is not available.");
        
        BuyerIdentifier buyerId = new(customerNumber, sessionId);
        
        Result<Buyer> getBuyerResult = await _repository.GetAsync<Buyer, string>(buyerId.ToString(), ct);
        Buyer buyer = getBuyerResult.UnwrapOrDefault(Buyer.Create(buyerId));

        buyer.AddCartItem(new CatalogueNumber(productCatalogueNumber));
        
        return await _repository.SaveAsync<Buyer, string>(buyer, ct);
    }
}