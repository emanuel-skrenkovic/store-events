﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Store.Core.Domain.Event;

namespace Store.Core.Domain;

public abstract class AggregateEntity<TKey>
{
    private readonly Dictionary<Type, Action<IEvent>> _eventAppliers;

    private readonly ICollection<IEvent> _events;
        
    public TKey Id { get; set; }

    public int Version { get; private set; }

    protected AggregateEntity()
    {
        _events = new List<IEvent>();
        _eventAppliers = new();
            
        RegisterAppliers();
    }
        
    public void ApplyEvent(IEvent domainEvent)
    {
        Type eventType = domainEvent.GetType();

        if (!_eventAppliers.ContainsKey(eventType))
        {
            throw new InvalidOperationException(
                $@"'Apply' method for event type {eventType.FullName} is not registered in aggregate {GetType().FullName}. 
Please call the 'RegisterApplier' in the 'RegisterAppliers' method in the aggregate implementation for the relevant event.'");
        }
                
                
        _eventAppliers[eventType](domainEvent);
        _events.Add(domainEvent);
    }

    public void Hydrate(TKey id, IReadOnlyCollection<IEvent> domainEvents)
    {
        if (domainEvents?.Any() != true) return;

        Id = id;
            
        foreach (IEvent domainEvent in domainEvents)
        {
            ApplyEvent(domainEvent);
            Version++;
        }

        // TODO: check if correct
        _events.Clear();
    }

    // TODO: should probably dequeue, i.e., clear the list.
    public IReadOnlyCollection<IEvent> GetUncommittedEvents() => _events.ToImmutableList();

    protected abstract void RegisterAppliers();
        
    protected void RegisterApplier<TEvent>(Action<TEvent> applier) where TEvent: IEvent
    {
        _eventAppliers.Add(typeof(TEvent), e => applier((TEvent)e));
    }
}