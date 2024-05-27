using BangchakAuthService.Services.RabbitMQ;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace BGTAuthService.Consumers
{
    public class ErrorConsumer : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IRabbitMQConnectionManager _connectionManager;

        public ErrorConsumer(IServiceProvider serviceProvider, IRabbitMQConnectionManager connectionManager)
        {
            _connectionManager = connectionManager;
            _serviceProvider = serviceProvider;
        }

        private void Run()
        {
            var channel = _connectionManager.GetChannel();

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (sender, e) =>
            {
                try
                {
                    var body = e.Body.ToArray();

                    var message = Encoding.UTF8.GetString(body);
                    var isError = JsonConvert.DeserializeObject<object>(message);

                    Console.WriteLine($"from other service error is {isError} userId {e.BasicProperties.CorrelationId}");
                    
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error processing message: {0}", ex.Message);
                }
            };

            channel.ExchangeDeclare(exchange: "akenarin.auth.error.ex", type: "direct", durable: true);
            channel.QueueDeclare(queue: "akenarin.auth.error.q", durable: true, exclusive: false, autoDelete: false, arguments: null);
            channel.QueueBind(queue: "akenarin.auth.error.q", exchange: "akenarin.auth.error.ex", routingKey: "auth-error");
            channel.BasicConsume("akenarin.auth.error.q", true, consumer);
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            Run();
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
