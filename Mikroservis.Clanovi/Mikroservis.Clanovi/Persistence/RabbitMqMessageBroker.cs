using RabbitMQ.Client;
using Newtonsoft.Json;
using System.Text;

namespace FitnessCentar.Members.Persistence
{
    public class RabbitMqMessageBroker : IMessageBroker
    {
         private readonly IConnection _connection;
         private readonly IModel _channel;

        public RabbitMqMessageBroker(IConfiguration configuration)
        {
            var factory = new ConnectionFactory
            {
                HostName = "rabbitmq",
                Port = 5672,
                UserName = "guest",
                Password = "guest"
            };
            _connection = factory.CreateConnection();
            
        }

        public bool Publish(string exchange, string routingKey, string message)
        {
            try
            {
                using var channel = _connection.CreateModel();

                // Deklaracija razmene
                channel.ExchangeDeclare(exchange: exchange, type: ExchangeType.Direct);

                // Deklaracija queue-a
                var queueName = channel.QueueDeclare().QueueName;
                channel.QueueBind(queue: queueName, exchange: exchange, routingKey: routingKey);

                var body = Encoding.UTF8.GetBytes(message);

                // Slanje poruke na RabbitMQ
                channel.BasicPublish(exchange: exchange, routingKey: routingKey, basicProperties: null, body: body);

                return true; // Uspešno poslata poruka
            }
            catch (Exception ex)
            {
                // Logovanje greške ili obrada u slučaju problema
                Console.WriteLine($"Error publishing message to RabbitMQ: {ex.Message}");
                return false; // Došlo je do greške prilikom slanja
            }
        }
    }

}
