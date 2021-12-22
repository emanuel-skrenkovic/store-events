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

    public ShippingInformation ShippingInformation { get; private set; }
        
    public decimal Total { get; private set; }
        
    public IReadOnlyCollection<OrderLine> OrderLines { get; private set; }
        
    public OrderStatus Status { get; private set; }
        
    public static Order Create(OrderNumber orderNumber, CustomerNumber customerNumber, OrderLines orderLines)
    {
        Order order = new();
        order.ApplyEvent(new OrderCreatedEvent(
            orderNumber.Value, 
            customerNumber.Value, 
            orderLines.Value));

        return order;
    }

    private void Apply(OrderCreatedEvent domainEvent)
    {
        Id = domainEvent.OrderId;
        CustomerNumber = domainEvent.CustomerNumber;
        OrderLines = domainEvent.OrderLines;
        Status = OrderStatus.Created;
    }
        
    public void SetShippingInformation(ShippingInformation shippingInformation)
    {
        ApplyEvent(new OrderShippingInformationSetEvent(Id, shippingInformation));
    }

    private void Apply(OrderShippingInformationSetEvent domainEvent)
    {
        ShippingInformation = domainEvent.ShippingInformation;
        Status = OrderStatus.ShippingInformationAdded;
    }

    protected override void RegisterAppliers()
    {
        RegisterApplier<OrderCreatedEvent>(Apply);
        RegisterApplier<OrderShippingInformationSetEvent>(Apply);
    }
}