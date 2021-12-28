using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Store.Core.Domain.ErrorHandling;

namespace Store.Core.Domain.Event;

public class IntegrationAggregateRepository : IAggregateRepository
{
    private readonly IAggregateRepository _repository;
    private readonly IIntegrationEventMapper _integrationEventMapper;
    private readonly IEventDispatcher _eventDispatcher;

    public IntegrationAggregateRepository(
        IAggregateRepository repository, 
        IIntegrationEventMapper integrationEventMapper, 
        IEventDispatcher eventDispatcher)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _integrationEventMapper = integrationEventMapper ?? throw new ArgumentNullException(nameof(integrationEventMapper));
        _eventDispatcher = eventDispatcher ?? throw new ArgumentNullException(nameof(eventDispatcher));
    }

    public Task<Result<T>> GetAsync<T, TKey>(TKey id) where T : AggregateEntity<TKey>, new()
        => _repository.GetAsync<T, TKey>(id);

    public async Task<Result> SaveAsync<T, TKey>(T entity) where T : AggregateEntity<TKey>
    {
        // Copy the events before commit to be able to translate
        // them to integration events after they are submitted.
        List<IEvent> events = new(entity.GetUncommittedEvents());
            
        Result saveResult = await _repository.SaveAsync<T, TKey>(entity);
           
        // TODO: think about how concurrency might mess up
        // temporal stream of events.
        foreach (IEvent @event in events)
        {
            if (_integrationEventMapper.TryMap(@event, out IEvent integrationEvent))
            {
                await _eventDispatcher.DispatchAsync(integrationEvent);
            }
        }

        return saveResult;
    }
}