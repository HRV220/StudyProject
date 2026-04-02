using StackExchange.Redis;
using System.Text.Json;

namespace OrderService
{
    public class OrderServ : IOrderService
    {
        private readonly ISubscriber _subscriber;

        public OrderServ(IConnectionMultiplexer redis)
        {
            _subscriber = redis.GetSubscriber();
        }
        public async Task CreateOrder(Order order)
        {
            var json = JsonSerializer.Serialize(order);
            await _subscriber.PublishAsync("order.created", json);
        }
    }
}
