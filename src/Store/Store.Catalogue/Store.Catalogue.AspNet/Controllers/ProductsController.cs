using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Store.Catalogue.Application.Product;
using Store.Catalogue.Application.Product.Command.AdjustPrice;
using Store.Catalogue.Application.Product.Command.Create;
using Store.Catalogue.Application.Product.Query.ProductDisplay;
using Store.Catalogue.AspNet.Models.Product;
using Store.Catalogue.Infrastructure.EntityFramework.Entity;

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
        public async Task<IActionResult> PostProduct([FromBody] ProductPostApiModel apiModel)
        {
            await _mediator.Send(new ProductCreateCommand(
                apiModel.Name, 
                apiModel.Price, 
                apiModel.Description));

            return StatusCode(201); // TODO: check best way
        }

        [HttpPost]
        [Route("{id:guid}/actions/adjust-price")]
        public async Task<IActionResult> AdjustProductPrice([FromRoute] Guid id, ProductPriceAdjustmentApiModel apiModel)
        {
            await _mediator.Send(new ProductAdjustPriceCommand(
                id, 
                apiModel.NewPrice, 
                apiModel.Reason));

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