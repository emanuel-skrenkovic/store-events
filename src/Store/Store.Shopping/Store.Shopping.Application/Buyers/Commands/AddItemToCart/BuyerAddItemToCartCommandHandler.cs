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

namespace Store.Shopping.Application.Buyers.Commands.AddItemToCart;

public class BuyerAddItemToCartCommandHandler : IRequestHandler<BuyerAddItemToCartCommand, Result>
{
    private readonly IBuyerRepository _buyerRepository;
    private readonly StoreShoppingDbContext _context;

    public BuyerAddItemToCartCommandHandler(IBuyerRepository buyerRepository, StoreShoppingDbContext context)
    {
        _buyerRepository = Ensure.NotNull(buyerRepository);
        _context         = Ensure.NotNull(context);
    }
        
    public async Task<Result> Handle(BuyerAddItemToCartCommand request, CancellationToken cancellationToken)
    {
        string productCatalogueNumber = request.ProductCatalogueNumber;
        
        string query =
            @"SELECT p.*
              FROM public.product p
              WHERE p.catalogue_number = @productCatalogueNumber;";

        IDbConnection db = _context.Database.GetDbConnection();

        ProductEntity product = await db.QuerySingleOrDefaultAsync<ProductEntity>(
            query, 
            new { productCatalogueNumber });

        if (product == null)    return new Error($"Could not find product with catalogue number: '{productCatalogueNumber}'.");
        if (!product.Available) return new Error($"Product with catalogue number '{productCatalogueNumber}' is not available.");
        
        BuyerIdentifier buyerId = new(request.CustomerNumber, request.SessionId);
        
        Result<Buyer> getBuyerResult = await _buyerRepository.GetBuyerAsync(buyerId);
        Buyer buyer = getBuyerResult.UnwrapOrDefault(Buyer.Create(buyerId));

        buyer.AddCartItem(new CatalogueNumber(request.ProductCatalogueNumber));
        
        return await _buyerRepository.SaveBuyerAsync(buyer);
    }
}