using EventBus;
using EventBus.Event;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SearchService.Models;
using System.Text;
using Typesense;

namespace SearchService.MessageHandlers
{
    public class PostCreateHandler(ILogger<PostCreateHandler> _logger, 
        IConnection _messageConnection, 
        ITypesenseClient _client) : BackgroundService
    {
        private IModel? _messageChannel;
        private EventingBasicConsumer consumer;

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _messageChannel = _messageConnection.CreateModel();
            _messageChannel.QueueDeclare(queue: Contanst.CreatePostQueue,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            consumer = new EventingBasicConsumer(_messageChannel);
            consumer.Received += ProcessMessage;

            _messageChannel.BasicConsume(queue: Contanst.CreatePostQueue,
                autoAck: true,
                consumer: consumer);

            return Task.CompletedTask;
        }
        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await base.StopAsync(cancellationToken);
            consumer.Received -= ProcessMessage;
            _messageChannel?.Dispose();
        }
        private void ProcessMessage(object? sender, BasicDeliverEventArgs args)
        {
            // Fire and forget fix using async in ProcessMessage
            _ = HandleMessageAsync(args);
        }

        private async Task HandleMessageAsync(BasicDeliverEventArgs args)
        {
            string message = Encoding.UTF8.GetString(args.Body.ToArray());
            _logger.LogInformation("Message retrieved from queue at {now}. Message Text: {text}", DateTime.Now, message);

            var messageData = System.Text.Json.JsonSerializer.Deserialize<CreatePost>(message);
            // Typesense không lưu DateTime dạng ISO string mà thường dùng Unix timestamp (số giây từ 1970).
            var created = new DateTimeOffset(messageData.CreatedAt).ToUnixTimeSeconds();

            var lastUpdateCreated = messageData.LastUpdatedAt != null ? new DateTimeOffset(messageData.LastUpdatedAt.Value).ToUnixTimeSeconds() : 0;

            var doc = new SearchPost
            {
                Id = messageData.Id,
                Title = messageData.Title,
                Content = messageData.Content,
                UserId = messageData.UserId,
                CreatedAt = created,
                LastUpdatedAt = lastUpdateCreated
            };
            try { 
                await _client.CreateDocument("posts", doc); 
            }
            catch(Exception ex)
            {
                _logger.LogError("Error when creating document in Typesense: {error}", ex.Message);
            }            
        }
    }
}
