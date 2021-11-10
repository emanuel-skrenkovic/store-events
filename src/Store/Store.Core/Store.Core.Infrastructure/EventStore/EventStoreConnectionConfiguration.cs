using EventStore.Client;

namespace Store.Core.Infrastructure.EventStore
{
    public record EventStoreConnectionConfiguration
    {
        public string SubscriptionId { get; set; }
        
        public UserCredentials Credentials { get; set; }
        
        public SubscriptionFilterOptions FilterOptions { get; set; } = new SubscriptionFilterOptions(EventTypeFilter.ExcludeSystemEvents());
    }
}