namespace SmartAccounting.EventBus.Configuration
{
    public interface IEventBusConfiguration
    {
        string ListenAndSendConnectionString { get; set; }
        string TopicName { get; set; }
        string Subscription { get; set; }
    }
}
