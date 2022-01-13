using System;
using System.Collections.Generic;
using System.Linq;
using Store.Core.Domain;
using Store.Core.Domain.ErrorHandling;
using Store.Shopping.Domain.Orders.Events;
using Store.Shopping.Domain.Orders.ValueObjects;
using Store.Shopping.Domain.ValueObjects;

namespace Store.Shopping.Domain.Orders;

public class Order : AggregateEntity<Guid>
{
    public string CustomerNumber { get; private set; }
    
    public Guid PaymentId { get; private set; }
    
    public ShippingInformation ShippingInformation { get; private set; }
        
    public decimal Total { get; private set; }
        
    public IReadOnlyCollection<OrderLine> OrderLines { get; private set; }
        
    public OrderStatus Status { get; private set; }
        
    public static Order Create(
        OrderNumber orderNumber, 
        CustomerNumber customerNumber, 
        OrderLines orderLines)
    {
        Order order = new();
        order.ApplyEvent(new OrderCreatedEvent(
            orderNumber.Value,
            customerNumber.Value,
            orderLines.Value,
            OrderStatus.Created));

        return order;
    }

    private void Apply(OrderCreatedEvent domainEvent)
    {
        Id = domainEvent.OrderId;
        CustomerNumber = domainEvent.CustomerNumber;
        
        OrderLines = domainEvent.OrderLines;
        Total = OrderLines.Select(ol => ol.Price).Sum();
        
        Status = domainEvent.Status;
    }

    public Result SubmitPayment(PaymentNumber paymentNumber)
    {
        if (PaymentId != default) return new Error($"Order with id {Id} already contains a submitted payment {PaymentId}.");
        ApplyEvent(new OrderPaymentSubmittedEvent(Id, paymentNumber.Value));
        
        return Result.Ok();
    }

    private void Apply(OrderPaymentSubmittedEvent domainEvent)
        => PaymentId = domainEvent.PaymentId;

    public void SetShippingInformation(ShippingInformation shippingInformation)
        => ApplyEvent(new OrderShippingInformationSetEvent(Id, shippingInformation.ToInfo()));

    private void Apply(OrderShippingInformationSetEvent domainEvent)
        => ShippingInformation = ShippingInformation.FromValues(domainEvent.ShippingInfo);

    public Result Confirm()
    {
        if (ShippingInformation == null) return new Error("Cannot confirm order without shipping info.");
        if (PaymentId == default)        return new Error("Cannot confirm order without a submitted payment.");
        
        ApplyEvent(new OrderConfirmedEvent(Id, OrderStatus.Confirmed));
        return Result.Ok();
    }

    private void Apply(OrderConfirmedEvent domainEvent)=> Status = domainEvent.Status;

    protected override void RegisterAppliers()
    {
        RegisterApplier<OrderCreatedEvent>(Apply);
        RegisterApplier<OrderPaymentSubmittedEvent>(Apply);
        RegisterApplier<OrderShippingInformationSetEvent>(Apply);
        RegisterApplier<OrderConfirmedEvent>(Apply);
    }
}