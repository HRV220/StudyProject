using Microsoft.AspNetCore.Mvc;

namespace OrderService
{
    public interface IOrderService
    {
        public Task CreateOrder(Order order);
    }
}
