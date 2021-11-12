using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Store.Core.Domain.Result;
using Store.Order.Application.Buyer.Commands.AddItemToCart;
using Store.Order.Application.Buyer.Commands.RemoveItemFromCart;
using Unit = Store.Core.Domain.Functional.Unit;

namespace Store.Order.AspNet.Controllers
{
    [ApiController]
    [Route("my")]
    public class ShoppingController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ShoppingController(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }
        
        [HttpPost]
        [Route("cart/actions/add-item")]
        public async Task<IActionResult> AddItemToCart(BuyerAddItemToCartCommand command)
        {
            Result<Unit> addItemToCartResult = await _mediator.Send(command); // TODO: need to pick up the user id from token or something.
            
            // TODO
            
            return Ok();
        }

        [HttpPost]
        [Route("cart/actions/remove-item")]
        public async Task<IActionResult> RemoveItemFromCart(BuyerRemoveItemFromCartCommand command)
        {
            Result<Unit> removeItemFromCartResult = await _mediator.Send(command); // TODO: need to pick up the user id from token or something.
            
            // TODO
            
            return Ok();
        }
    }
}