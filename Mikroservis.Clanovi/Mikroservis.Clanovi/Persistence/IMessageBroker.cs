namespace FitnessCentar.Members.Persistence
{
    public interface IMessageBroker
    {
        bool Publish(string exchange, string routingKey, string message);
    }

}
