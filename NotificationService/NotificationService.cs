using StackExchange.Redis;
using System.Text.Json;

namespace NotificationService
{
    public class NotificationSubscriber : BackgroundService
    {
        private readonly ISubscriber _subscriber;
        private readonly ILogger<NotificationSubscriber> _logger;

        public NotificationSubscriber(IConnectionMultiplexer redis, ILogger<NotificationSubscriber> logger)
        {
            _subscriber = redis.GetSubscriber();
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _subscriber.SubscribeAsync("order.created", (channel, message) =>
            {
                var order = JsonSerializer.Deserialize<Order>((string)message);
                _logger.LogInformation($"Получено событие: OrderId={order.Id}, Name={order.Name}");
            });

            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
    }
}
