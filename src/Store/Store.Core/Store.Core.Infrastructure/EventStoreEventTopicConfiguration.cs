using EventStore.Client;

namespace Store.Core.Infrastructure
{
    public class EventStoreEventTopicConfiguration
    {
        public string SubscriptionId { get; set; }
        
        public UserCredentials Credentials { get; set; }
        
        public SubscriptionFilterOptions FilterOptions { get; set; } = new SubscriptionFilterOptions(EventTypeFilter.ExcludeSystemEvents());
    }
}