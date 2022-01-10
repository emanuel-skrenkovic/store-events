using MediatR;
using Microsoft.AspNetCore.Mvc;
using Store.Core.Domain;
using Store.Core.Domain.ErrorHandling;
using Store.Core.Infrastructure.AspNet;
using Store.Inventory.Application.Commands;

namespace Store.Inventory.AspNet.Controllers;

[ApiController]
[Route("inventory")]
public class InventoryController: ControllerBase
{
    private readonly IMediator _mediator;

    public InventoryController(IMediator mediator)
        => _mediator = Ensure.NotNull(mediator);

    [HttpPost]
    [Route("{productId:guid}/actions/add")]
    public async Task<IActionResult> AddToStock([FromRoute] Guid productId, ProductInventoryAddToStockCommand command)
    {
        Result result = await _mediator.Send(command with { ProductId = productId });
        return result.Match(Ok, this.HandleError);
    }
    
    [HttpPost]
    [Route("{productId:guid}/actions/remove")]
    public async Task<IActionResult> RemoveFromStock([FromRoute] Guid productId, ProductInventoryRemoveFromStockCommand command)
    {
        Result result = await _mediator.Send(command with { ProductId = productId });
        return result.Match(Ok, this.HandleError);
    }
    
    [HttpGet]
    [Route("{productId:guid}")]
    public Task<IActionResult> GetProductInventoryState([FromRoute] Guid productId) => throw new NotImplementedException();
}