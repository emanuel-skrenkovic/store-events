using System;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Store.Core.Domain;
using Store.Core.Domain.ErrorHandling;
using Store.Payments.Application.Payments.Commands;
using Store.Payments.Domain.Payments;
using Store.Payments.Domain.Payments.Events;
using Xunit;

namespace Store.Payments.Tests.Integration;

public class PaymentEventStoreTests : IClassFixture<PaymentsEventStoreFixture>
{
    private readonly PaymentsEventStoreFixture _fixture;

    public PaymentEventStoreTests(PaymentsEventStoreFixture fixture)
        => _fixture = Ensure.NotNull(fixture);

    [Fact]
    public async Task PaymentCreateCommand_Should_CreatePayment()
    {
        var mediator = _fixture.GetService<IMediator>();

        Guid orderId = Guid.NewGuid();
        string source = Guid.NewGuid().ToString();
        decimal amount = 15m;
        string note = Guid.NewGuid().ToString();

        #region Act
        
        PaymentCreateCommand command = new(orderId, source, amount, note);
        Result<PaymentCreateResponse> paymentCreateResult = await mediator.Send(command);
        
        #endregion
        
        #region Assert
        
        Assert.NotNull(paymentCreateResult);
        Assert.True(paymentCreateResult.IsOk);

        PaymentCreateResponse response = paymentCreateResult.UnwrapOrDefault();
        
        Assert.NotNull(response);
        Assert.NotEqual(default, response.PaymentId);

        var events = await _fixture.EventStoreFixture.Events($"{typeof(Payment).FullName}-{response.PaymentId}");
        Assert.NotEmpty(events);
        Assert.Contains(events, e => e is PaymentCreatedEvent);

        var @event = events.SingleOrDefault(e => e is PaymentCreatedEvent) as PaymentCreatedEvent;
        Assert.NotNull(@event);
        Assert.Equal(response.PaymentId, @event.PaymentId);
        Assert.Equal(orderId, @event.OrderId);
        Assert.Equal(source, @event.Source);
        Assert.Equal(amount, @event.Amount);
        Assert.Equal(note, @event.Note);
        
        #endregion
    }

    [Fact]
    public async Task PaymentRefundCommand_Should_Refund()
    {
        var mediator = _fixture.GetService<IMediator>();
        
        #region Preconditions

        Guid orderId = Guid.NewGuid();
        string source = Guid.NewGuid().ToString();
        decimal amount = 15m;
        string note = Guid.NewGuid().ToString();

        string refundNote = Guid.NewGuid().ToString();
        
        Guid paymentId = await _fixture.PaymentExists(orderId, source, amount, note);

        #endregion
        
        #region Act

        PaymentRefundCommand command = new(paymentId, refundNote);
        Result<PaymentRefundResponse> refundResult = await mediator.Send(command);

        #endregion
        
        #region Assert
        
        Assert.NotNull(refundResult);
        Assert.True(refundResult.IsOk);

        var response = refundResult.UnwrapOrDefault();
        Assert.NotNull(response);
        Assert.NotEqual(default, response.RefundId);
        
        var events = await _fixture.EventStoreFixture.Events($"{typeof(Payment).FullName}-{paymentId}");
        Assert.NotEmpty(events);
        Assert.Contains(events, e => e is PaymentRefundedEvent);

        var @event = events.SingleOrDefault(e => e is PaymentRefundedEvent) as PaymentRefundedEvent;
        Assert.NotNull(@event);
        Assert.Equal(paymentId, @event.PaymentId);
        Assert.Equal(response.RefundId, @event.Refund.Id);
        Assert.Equal(refundNote, @event.Refund.Note);
        Assert.Equal(amount, @event.Refund.Amount);
        
        #endregion
    }
    
    [Fact]
    public async Task PaymentRefundCommand_Should_ReturnOk_When_PaymentAlreadyRefunded()
    {
        var mediator = _fixture.GetService<IMediator>();
        
        #region Preconditions

        Guid orderId = Guid.NewGuid();
        string source = Guid.NewGuid().ToString();
        decimal amount = 15m;
        string note = Guid.NewGuid().ToString();

        string refundNote = Guid.NewGuid().ToString();
        
        Guid paymentId = await _fixture.PaymentExists(orderId, source, amount, note);
        await mediator.Send(new PaymentRefundCommand(paymentId, refundNote));

        #endregion
        
        #region Act

        PaymentRefundCommand command = new(paymentId, refundNote);
        Result<PaymentRefundResponse> refundResult = await mediator.Send(command);

        #endregion
        
        #region Assert
        
        Assert.NotNull(refundResult);
        Assert.True(refundResult.IsOk);

        var response = refundResult.UnwrapOrDefault();
        Assert.NotNull(response);
        Assert.NotEqual(default, response.RefundId);
        
        var events = await _fixture.EventStoreFixture.Events($"{typeof(Payment).FullName}-{paymentId}");
        Assert.NotEmpty(events);
        Assert.Contains(events, e => e is PaymentRefundedEvent);

        var @event = events.SingleOrDefault(e => e is PaymentRefundedEvent) as PaymentRefundedEvent;
        Assert.NotNull(@event);
        Assert.Equal(paymentId, @event.PaymentId);
        Assert.Equal(response.RefundId, @event.Refund.Id);
        Assert.Equal(refundNote, @event.Refund.Note);
        Assert.Equal(amount, @event.Refund.Amount);
        
        #endregion
    }

    [Fact]
    public async Task PaymentVerifyCommand_Should_Verify()
    {
        var mediator = _fixture.GetService<IMediator>();
        
        #region Preconditions

        Guid orderId = Guid.NewGuid();
        string source = Guid.NewGuid().ToString();
        decimal amount = 15m;
        string note = Guid.NewGuid().ToString();
        
        Guid paymentId = await _fixture.PaymentExists(orderId, source, amount, note);
        
        #endregion
        
        #region Act

        PaymentVerifyCommand command = new(paymentId);
        Result paymentVerifyResult = await mediator.Send(command);

        #endregion
        
        #region Assert
        
        Assert.NotNull(paymentVerifyResult);
        Assert.True(paymentVerifyResult.IsOk);

        var events = await _fixture.EventStoreFixture.Events($"{typeof(Payment).FullName}-{paymentId}");
        Assert.NotEmpty(events);
        Assert.Contains(events, e => e is PaymentVerifiedEvent);
        
        var @event = events.SingleOrDefault(e => e is PaymentVerifiedEvent) as PaymentVerifiedEvent;
        Assert.NotNull(@event);
        
        Assert.Equal(paymentId, @event.PaymentId);
        Assert.Equal(PaymentStatus.Verified, @event.Status);
        
        #endregion
    }

    [Fact]
    public async Task PaymentVerifyCommand_Should_ReturnError_When_PaymentRefunded()
    {
        var mediator = _fixture.GetService<IMediator>();
        
        #region Preconditions

        Guid orderId = Guid.NewGuid();
        string source = Guid.NewGuid().ToString();
        decimal amount = 15m;
        string note = Guid.NewGuid().ToString();
        
        Guid paymentId = await _fixture.PaymentExists(orderId, source, amount, note);
        await mediator.Send(new PaymentRefundCommand(paymentId));
        
        #endregion
        
        #region Act

        PaymentVerifyCommand command = new(paymentId);
        Result paymentVerifyResult = await mediator.Send(command);

        #endregion 
        
        #region Assert
        
        Assert.NotNull(paymentVerifyResult);
        Assert.True(paymentVerifyResult.IsError);

        var events = await _fixture.EventStoreFixture.Events($"{typeof(Payment).FullName}-{paymentId}");
        Assert.NotEmpty(events);
        Assert.DoesNotContain(events, e => e is PaymentVerifiedEvent);

        #endregion
    }
}