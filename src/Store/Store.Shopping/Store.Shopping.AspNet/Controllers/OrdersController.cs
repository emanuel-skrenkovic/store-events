using MediatR;
using Microsoft.AspNetCore.Mvc;
using Store.Core.Domain.ErrorHandling;
using Store.Core.Infrastructure.AspNet;
using Store.Shopping.Application.Orders.Commands.CreateOrder;
using Store.Shopping.Application.Orders.Queries;
using Store.Shopping.Infrastructure.Entity;

namespace Store.Shopping.AspNet.Controllers;

[ApiController]
[Route("[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IMediator _mediator;

    public OrdersController(IMediator mediator)
        => _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));

    #region Actions
        
    [HttpPut]
    [Route("actions/place-order")]
    public async Task<IActionResult> PlaceOrderAsync([FromBody] OrderCreateCommand command)
    {
        Result<OrderCreateResponse> placeOrderResult = await _mediator.Send(command);

        return placeOrderResult.Match(
            response => CreatedAtAction("GetOrder", new { id = response.OrderId }, null),
            this.HandleError);
    }
        
    #endregion

    [HttpGet]
    [Route("id:guid")]
    public async Task<IActionResult> GetOrder([FromRoute] Guid id)
    {
        Result<Order> getOrderResult = await _mediator.Send(new GetOrderQuery(id));
        return getOrderResult.Match(Ok, this.HandleError);
    } 
}