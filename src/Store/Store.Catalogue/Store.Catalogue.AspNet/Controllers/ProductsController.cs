using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Store.Catalogue.Application.Product.Command.AdjustPrice;
using Store.Catalogue.Application.Product.Command.Create;
using Store.Catalogue.AspNet.Models.Product;

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
        
        #region Commands
        
        [HttpPost]
        [Route("commands/create")]
        public async Task<IActionResult> PostProduct([FromBody] ProductPostApiModel apiModel)
        {
            await _mediator.Send(new ProductCreateCommand(
                apiModel.Name, 
                apiModel.Price, 
                apiModel.Description));

            return StatusCode(201); // TODO: check best way
        }

        [HttpPost]
        [Route("{id:guid}/commands/adjust-price")]
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
        public Task<IActionResult> GetProduct([FromRoute] Guid id)
        {
            throw new NotImplementedException();
        }
    }
}