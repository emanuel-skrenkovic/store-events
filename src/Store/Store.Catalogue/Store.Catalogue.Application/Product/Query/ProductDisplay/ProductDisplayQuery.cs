using System;
using MediatR;

namespace Store.Catalogue.Application.Product.Query.ProductDisplay;

public record ProductDisplayQuery(Guid Id) : IRequest<ProductDto>;