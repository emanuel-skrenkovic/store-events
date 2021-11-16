using System;
using System.Collections.Generic;
using Store.Core.Domain;
using Store.Order.Domain.Orders.Events;

namespace Store.Order.Domain.Orders
{
    public class Order : AggregateEntity<Guid>
    {
        public string CustomerNumber { get; private set; }

        public ShippingInformation ShippingInformation { get; private set; }
        
        public decimal Amount { get; private set; }
        
        public Dictionary<CatalogueNumber, OrderLine> OrderLines { get; private set; }
        
        public OrderStatus Status { get; private set; }
        
        public static Order Create(Guid id, string customerNumber)
        {
            Order order = new();
            order.ApplyEvent(new OrderCreatedEvent(id, customerNumber));

            return order;
        }

        private void Apply(OrderCreatedEvent domainEvent)
        {
            Id = domainEvent.EntityId;
            CustomerNumber = domainEvent.CustomerNumber;
            OrderLines = new();
            Status = OrderStatus.Created;
        }

        public void AddOrderLine(OrderLine orderLine)
        {
            ApplyEvent(new OrderOrderLineAddedEvent(Id, orderLine));
        }

        private void Apply(OrderOrderLineAddedEvent domainEvent)
        {
            OrderLines.Add(domainEvent.OrderLine.Item.CatalogueNumber, domainEvent.OrderLine);
        }

        public void SetShippingInformation(ShippingInformation shippingInformation)
        {
            // TODO: dunno, looks ugly
            if (ShippingInformation == null)
            {
                ApplyEvent(new OrderShippingInformationAddedEvent(Id, shippingInformation));
            }
            else
            {
                ApplyEvent(new OrderShippingInformationChangedEvent(Id, shippingInformation));
            }
        }

        private void Apply(OrderShippingInformationAddedEvent domainEvent)
        {
            ShippingInformation = domainEvent.ShippingInformation;
            Status = OrderStatus.ShippingInformationAdded;
        }

        private void Apply(OrderShippingInformationChangedEvent domainEvent)
        {
            ShippingInformation = domainEvent.ShippingInformation;
            Status = OrderStatus.ShippingInformationAdded;
        }

        protected override void RegisterAppliers()
        {
            RegisterApplier<OrderCreatedEvent>(Apply);
            RegisterApplier<OrderOrderLineAddedEvent>(Apply);
            RegisterApplier<OrderShippingInformationAddedEvent>(Apply);
            RegisterApplier<OrderShippingInformationChangedEvent>(Apply);
        }
    }
}