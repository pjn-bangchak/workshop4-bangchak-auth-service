using RabbitMQ.Client;

namespace BangchakAuthService.Services.RabbitMQ
{
    public interface IRabbitMQConnectionManager
    {
            IConnection GetConnection();
            IModel GetChannel();
    }
}
