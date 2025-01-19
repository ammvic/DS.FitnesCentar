using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using FitnessCentar.Email.Services;
using FitnessCentar.Email.Models;

namespace FitnessCentar.Email.Services
{
    public class RabbitMqListener
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly EmailService _emailService;

        public RabbitMqListener(IConfiguration configuration, EmailService emailService)
        {
            var factory = new ConnectionFactory
            {
                HostName = "rabbitmq",
                Port = 5672,
                UserName = "guest",
                Password = "guest"
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _emailService = emailService;
        }

        public void StartListening()
        {
            _channel.ExchangeDeclare("email_exchange", ExchangeType.Direct);
            var queueName = _channel.QueueDeclare().QueueName;
            _channel.QueueBind(queueName, "email_exchange", "email_key");

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                // Deserializacija poruke
                var emailRequest = JsonConvert.DeserializeObject<EmailRequest>(message);

                if (emailRequest != null)
                {
                    // Slanje email-a putem EmailService
                    var result = await _emailService.SendEmailAsync(emailRequest.Recipient, emailRequest.Subject, emailRequest.Body);

                    if (result)
                    {
                        Console.WriteLine($"Email poslat na: {emailRequest.Recipient}");
                    }
                    else
                    {
                        Console.WriteLine($"Greška prilikom slanja emaila na: {emailRequest.Recipient}");
                    }
                }
            };

            _channel.BasicConsume(queueName, true, consumer);
            Console.WriteLine("Slušalac aktiviran. Čeka se na poruke...");
        }
    }
}
