using ChatService.Hubs;
using Microsoft.AspNetCore.SignalR;
using System.Text;
using EventBus;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SharedObject;


namespace ChatService.MessageHandlers
{
    public class RealtimeCommentHandler(ILogger<RealtimeCommentHandler> logger, 
        IHubContext<ChatHub> hub, IConnection _messageConnection) : BackgroundService
    {
        private IModel? _messageChannel;
        private EventingBasicConsumer consumer;

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _messageChannel = _messageConnection.CreateModel();
            _messageChannel.QueueDeclare(queue: Contanst.RealtimeCommentQueue,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            consumer = new EventingBasicConsumer(_messageChannel);
            consumer.Received += ProcessMessage;

            _messageChannel.BasicConsume(queue: Contanst.RealtimeCommentQueue,
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
            logger.LogInformation("Message retrieved from queue at {now}. Message Text: {text}", DateTime.Now, message);
            var cmtDto = System.Text.Json.JsonSerializer.Deserialize<CommentDto>(message);
            await hub.Clients.All.SendAsync("AddComment", cmtDto);
        }
    }
}
