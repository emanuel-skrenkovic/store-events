using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Store.Core.Domain.Result;
using Store.Order.Application.Order.Command.AddShippingInformation;
using Store.Order.Application.Order.Command.PlaceOrder;
using Unit = Store.Core.Domain.Functional.Unit;

namespace Store.Order.AspNet.Controllers
{
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
            Result<Unit> placeOrderResult = await _mediator.Send(command);

            // TODO: need to return either 201 if created, or 200 if already exists.
            return Ok();
        }

        [HttpPut]
        [Route("{orderId:guid}/actions/set-shipping-information")] // TODO route
        public async Task<IActionResult> SetOrderShippingInformation([FromRoute] Guid orderId, [FromBody] OrderAddShippingInformationCommand command)
        {
            Result<Unit> placeOrderResult = await _mediator.Send(command with { OrderNumber = orderId });

            return Ok();
        }
        
        #endregion
    }
}