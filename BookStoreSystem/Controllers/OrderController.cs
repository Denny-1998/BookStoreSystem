using BookStoreSystem.Services;
using Microsoft.AspNetCore.Mvc;
using BookStoreSystem.Model;

namespace BookStoreSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly OrderService _orderService;

        public OrderController(OrderService orderService)
        {
            _orderService = orderService;
        }



        [HttpPost]
        public async Task<ActionResult<Order>> CreateOrder(CreateOrderDTO orderDto)
        {
            var order = new Order
            {
                CustomerId = orderDto.CustomerId,
                TotalAmount = orderDto.TotalAmount,
                OrderDate = DateTime.UtcNow,
                OrderItems = orderDto.OrderItems.Select(item => new OrderItem
                {
                    BookId = item.BookId,
                    Quantity = item.Quantity
                }).ToList()
            };

            var createdOrder = await _orderService.CreateOrder(order);
            return CreatedAtAction(nameof(GetOrder), new { id = createdOrder.Id }, createdOrder);
        }



        [HttpGet("recent")]
        public async Task<ActionResult<List<Model.Order>>> GetRecentOrders()
        {
            var orders = await _orderService.GetRecentOrders();
            return Ok(orders);
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<Model.Order>> GetOrder(int id)
        {
            var order = await _orderService.GetOrder(id);
            if (order == null)
                return NotFound();

            return Ok(order);
        }
    }

    public class CreateOrderDTO
    {
        public int CustomerId { get; set; }
        public decimal TotalAmount { get; set; }
        public List<CreateOrderItemDTO> OrderItems { get; set; } = new();
    }

    public class CreateOrderItemDTO
    {
        public int BookId { get; set; }
        public int Quantity { get; set; }
    }

}
