using System;

namespace Store.Catalogue.AspNet.Models;

public record ProductApiModel(
    Guid Id, 
    DateTime CreatedAt, 
    DateTime UpdatedAt, 
    string Name, 
    decimal Price, 
    bool Available, 
    string Description = null);