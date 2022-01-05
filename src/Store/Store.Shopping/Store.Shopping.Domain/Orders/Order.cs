using System;
using System.Collections.Generic;
using Store.Core.Domain;
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
        PaymentNumber paymentNumber, 
        OrderLines orderLines, 
        ShippingInformation shippingInformation)
    {
        Order order = new();
        order.ApplyEvent(new OrderPlacedEvent(
            orderNumber.Value, 
            customerNumber.Value, 
            paymentNumber.Value,
            orderLines.Value,
            shippingInformation.ToInfo()));

        return order;
    }

    private void Apply(OrderPlacedEvent domainEvent)
    {
        Id = domainEvent.OrderId;
        CustomerNumber = domainEvent.CustomerNumber;
        PaymentId = domainEvent.PaymentId;
        OrderLines = domainEvent.OrderLines;
        Status = OrderStatus.Created;
        ShippingInformation = ShippingInformation.FromValues(domainEvent.ShippingInfo);
    }

    protected override void RegisterAppliers()
    {
        RegisterApplier<OrderPlacedEvent>(Apply);
    }
}