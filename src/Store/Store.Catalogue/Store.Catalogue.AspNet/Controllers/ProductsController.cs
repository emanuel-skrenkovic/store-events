using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Store.Catalogue.Application.Product;
using Store.Catalogue.Application.Product.Command.AdjustPrice;
using Store.Catalogue.Application.Product.Command.Availability;
using Store.Catalogue.Application.Product.Command.Create;
using Store.Catalogue.Application.Product.Command.Rename;
using Store.Catalogue.Application.Product.Query.ProductDisplay;
using Store.Core.Domain.ErrorHandling;
using Unit = Store.Core.Domain.Functional.Unit;

namespace Store.Catalogue.AspNet.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProductsController(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }
        
        #region Actions 
        
        [HttpPost]
        [Route("actions/create")]
        public async Task<IActionResult> PostProduct([FromBody] ProductCreateCommand command)
        {
            Result<Guid> createProductResult = await _mediator.Send(command);

            return createProductResult.Match<IActionResult>(
                createdProductId => CreatedAtAction("GetProduct", new { id = createdProductId }, null),
                _                => BadRequest()); // TODO
        }

        [HttpPost]
        [Route("{id:guid}/actions/adjust-price")]
        public async Task<IActionResult> AdjustProductPrice([FromRoute] Guid id, ProductAdjustPriceCommand command)
        {
            await _mediator.Send(command with { ProductId = id });

            return Ok();
        }

        [HttpPost]
        [Route("{id:guid}/actions/rename")]
        public async Task<IActionResult> RenameProduct([FromRoute] Guid id, [FromBody] ProductRenameCommand command)
        {
            Result<Unit> _ = await _mediator.Send(command with { ProductId = id });

            // TODO: handle errors
            return Ok();
        }

        [HttpPost]
        [Route("{id:guid}/actions/set-availability")]
        public async Task<IActionResult> SetProductAvailability([FromRoute] Guid id, [FromBody] ProductSetAvailabilityCommand command)
        {
            Result<Unit> _ = await _mediator.Send(command with { ProductId = id });

            // TODO: handle errors.
            return Ok();
        }
        
        #endregion

        [HttpGet]
        [Route("{id:guid}")]
        public async Task<IActionResult> GetProduct([FromRoute] Guid id)
        {
            ProductDto product = await _mediator.Send(new ProductDisplayQuery(id));

            if (product == null) return NotFound();

            return Ok(product);
        }
    }
}