using MediatR;
using Microsoft.AspNetCore.Mvc;
using Store.Core.Domain.ErrorHandling;
using Store.Core.Infrastructure.AspNet;
using Store.Shopping.Application.Orders.Commands.AddShippingInformation;
using Store.Shopping.Application.Orders.Commands.PlaceOrder;

namespace Store.Shopping.AspNet.Controllers;

[ApiController]
[Route("[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IMediator _mediator;

    public OrdersController(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    #region Actions
        
    [HttpPut]
    [Route("actions/place-order")]
    public async Task<IActionResult> PlaceOrderAsync([FromBody] OrderPlaceCommand command)
    {
        Result<OrderPlaceResponse> placeOrderResult = await _mediator.Send(command);

        return placeOrderResult.Match(
            response => CreatedAtAction("GetOrder", new { id = response.OrderId }, null),
            this.HandleError);
    }

    [HttpPut]
    [Route("{orderId:guid}/actions/set-shipping-information")] // TODO route
    public async Task<IActionResult> SetOrderShippingInformation([FromRoute] Guid orderId, [FromBody] OrderAddShippingInformationCommand command)
    {
        Result placeOrderResult = await _mediator.Send(command with { OrderId = orderId });

        return Ok();
    }
        
    #endregion

    [HttpGet]
    [Route("id:guid")]
    public Task<IActionResult> GetOrder() => throw new NotImplementedException();
}