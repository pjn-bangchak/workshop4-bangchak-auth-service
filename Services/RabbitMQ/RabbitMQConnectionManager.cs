using RabbitMQ.Client;

namespace BangchakAuthService.Services.RabbitMQ
{
    public class RabbitMQConnectionManager : IRabbitMQConnectionManager, IDisposable
    {
        private readonly IConfiguration _configuration;
        private IConnection _connection = null!;
        private IModel _channel = null!;

        public RabbitMQConnectionManager(IConfiguration configuration)
        {
            _configuration = configuration;
            CreateConnection();
            CreateChannel();
        }

        private void CreateConnection()
        {
            var factory = new ConnectionFactory()
            {
                Uri = new Uri(_configuration.GetSection("RABBITMQ_URI").Value!.ToString())
            };
            _connection = factory.CreateConnection();
        }

        private void CreateChannel()
        {
            _channel = _connection.CreateModel();
        }

        public IConnection GetConnection() => _connection;

        public IModel GetChannel() => _channel;

        public void Dispose()
        {
            _channel?.Close();
            _connection?.Close();
            _channel?.Dispose();
            _connection?.Dispose();
        }
    }
}
