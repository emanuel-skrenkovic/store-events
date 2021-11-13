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
        
        #region Actions
        
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
        
        #endregion

        [HttpGet]
        [Route("cart")]
        public Task<IActionResult> GetCart()
        {
            throw new NotImplementedException();
            /*
            CartView cart = await _mediator.Send(new BuyerCartGetQuery());

            if (cart == null)
            {
                return NotFound();
            }
            
            return Ok(cart);
            */
        }
    }
}