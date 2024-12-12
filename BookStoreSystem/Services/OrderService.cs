using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using System.Text.Json;
using BookStoreSystem.Model;
using System.Text.Json.Serialization;

namespace BookStoreSystem.Services
{
    public class OrderService
    {
        private readonly BookStoreDbContext _context;
        private readonly IConnectionMultiplexer _redis;
        private const string RecentOrdersKey = "recent_orders";
        private readonly JsonSerializerOptions _jsonOptions;

        public OrderService(BookStoreDbContext context, IConnectionMultiplexer redis)
        {
            _context = context;
            _redis = redis;
            _jsonOptions = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.IgnoreCycles
            };
        }

        public async Task<Model.Order> CreateOrder(Model.Order order)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Check and update book inventory
                foreach (var item in order.OrderItems)
                {
                    var book = await _context.Books.FindAsync(item.BookId);
                    if (book == null)
                        throw new Exception($"Book with ID {item.BookId} not found");

                    if (book.StockQuantity < item.Quantity)
                        throw new Exception($"Insufficient stock for book {book.Title}");

                    book.StockQuantity -= item.Quantity;
                }

                order.OrderDate = DateTime.UtcNow;

                // Save order
                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                // Add to Redis recent orders list
                var redis = _redis.GetDatabase();
                await redis.ListLeftPushAsync(RecentOrdersKey, JsonSerializer.Serialize(order, _jsonOptions));
                await redis.ListTrimAsync(RecentOrdersKey, 0, 9);

                await transaction.CommitAsync();
                return order;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<List<Model.Order>> GetRecentOrders()
        {
            var redis = _redis.GetDatabase();

            // recent orders from Redis
            var recentOrders = await redis.ListRangeAsync(RecentOrdersKey);

            if (recentOrders.Length > 0)
            {
                return recentOrders.Select(o =>
                    JsonSerializer.Deserialize<Model.Order>(o!, _jsonOptions)!).ToList();
            }

            // If Redis is empty, get from SQL 
            var orders = await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderItems)
                .OrderByDescending(o => o.OrderDate)
                .Take(10)
                .ToListAsync();

            // Repopulate Redis
            await redis.KeyDeleteAsync(RecentOrdersKey);
            foreach (var order in orders.AsEnumerable().Reverse())
            {
                await redis.ListLeftPushAsync(RecentOrdersKey, JsonSerializer.Serialize(order, _jsonOptions));
            }

            return orders;
        }

        public async Task<Model.Order?> GetOrder(int id)
        {
            return await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.Id == id);
        }
    }


}
