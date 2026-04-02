using Microsoft.AspNetCore.Mvc;

namespace OrderService
{
    [ApiController]
    [Route("[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Order order)
        {
            await _orderService.CreateOrder(order);

            return Ok();
        }
    }
}
