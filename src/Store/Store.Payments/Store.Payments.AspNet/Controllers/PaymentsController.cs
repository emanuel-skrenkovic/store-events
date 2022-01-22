using MediatR;
using Microsoft.AspNetCore.Mvc;
using Store.Core.Domain;
using Store.Core.Domain.ErrorHandling;
using Store.Core.Infrastructure.AspNet;
using Store.Payments.Application.Payments.Commands;

namespace Store.Payments.AspNet.Controllers;

[ApiController]
[Route("[controller]")]
public class PaymentsController : ControllerBase
{
    private readonly IMediator _mediator;

    public PaymentsController(IMediator mediator)
        => _mediator = Ensure.NotNull(mediator);
    
    #region Actions

    [HttpPut]
    [Route("actions/create")]
    [ProducesResponseType(200)] // TODO: need 200 in case it already exists
    [ProducesResponseType(201)] 
    [ProducesResponseType(typeof(ProblemDetails), 400)]
    [ProducesResponseType(typeof(ProblemDetails), 500)]
    public async Task<IActionResult> CreatePayment([FromBody] PaymentCreateCommand command)
    {
        Result<PaymentCreateResponse> result = await _mediator.Send(command);
        return result.Match(
            response => CreatedAtAction("GetPayment", new { response.PaymentId }, null), 
            this.HandleError);
    }

    [HttpPut]
    [Route("{paymentId:guid}/actions/refund")]
    [ProducesResponseType(200)]
    [ProducesResponseType(typeof(ProblemDetails), 400)]
    [ProducesResponseType(typeof(ProblemDetails), 500)]
    public async Task<IActionResult> RefundPayment([FromRoute] Guid paymentId, [FromBody] PaymentRefundCommand command)
    {
        Result<PaymentRefundResponse> result = await _mediator.Send(command with { PaymentId = paymentId });
        return result.Match(Ok, this.HandleError);
    }

    [HttpPut]
    [Route("{paymentId:guid}/actions/complete")]
    [ProducesResponseType(200)]
    [ProducesResponseType(typeof(ProblemDetails), 400)]
    [ProducesResponseType(typeof(ProblemDetails), 500)]
    public async Task<IActionResult> CompletePayment([FromRoute] Guid paymentId, [FromBody] PaymentVerifyCommand command)
    {
        Result result = await _mediator.Send(command with { PaymentId = paymentId });
        return result.Match(Ok, this.HandleError);
    }
    
    #endregion

    [HttpGet]
    [Route("{paymentId:guid}")]
    public Task<IActionResult> GetPayment([FromRoute] Guid paymentId) => throw new NotImplementedException();
}