using RabbitMQ.Client;

namespace SharedObject
{
    public interface IBusRabbitmqService
    {
        Task PublishAsync<T>(string queueName, T message) where T : class;
    }
    public class BusRabbitmqService(IConnection connection) : IBusRabbitmqService
    {
        public Task PublishAsync<T>(string queueName, T message) where T : class
        {
            using var channel = connection.CreateModel();
            var props = channel.CreateBasicProperties();
            props.Persistent = true;//giup service ofline luu tin nhan lai và send khi online

            channel.QueueDeclare(queue: queueName,
                                 durable: true,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            var body = System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(message);

            channel.BasicPublish(exchange: "",
                                 routingKey: queueName,
                                 basicProperties: props,
                                 body: body);
            return Task.CompletedTask;
        }
    }
}
