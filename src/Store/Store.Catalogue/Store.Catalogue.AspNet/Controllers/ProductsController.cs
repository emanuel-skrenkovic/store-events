using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
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
        
        [HttpPost]
        public async Task<IActionResult> PostProduct([FromBody] ProductPostApiModel apiModel)
        {
            await _mediator.Send(new ProductCreateCommand(
                apiModel.Name, 
                apiModel.Price, 
                apiModel.Description));
            
            return Ok();
        }
    }
}