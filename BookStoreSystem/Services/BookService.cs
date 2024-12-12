using BookStoreSystem.Model;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using StackExchange.Redis;

namespace BookStoreSystem.Services
{
    public class BookService
    {
        private readonly BookStoreDbContext _dbContext;
        private readonly IConnectionMultiplexer _redis;

        public BookService(BookStoreDbContext dbContext, IConnectionMultiplexer redis)
        {
            _dbContext = dbContext;
            _redis = redis;
        }

        public async Task<List<Book>> GetAllBooks()
        {
            return await _dbContext.Books.ToListAsync();
        }

        public async Task<Book?> GetBook(int id)
        {
            var redis = _redis.GetDatabase();
            var cacheKey = $"book:{id}";

            var cachedBook = await redis.StringGetAsync(cacheKey);
            if (cachedBook.HasValue)
            {
                return JsonSerializer.Deserialize<Book>(cachedBook!);
            }

            var book = await _dbContext.Books.FindAsync(id);

            // Sleep for 5 seconds to test, if the cache is working 
            //Thread.Sleep(5000);
            
            if (book == null)
            {
                return null;
            }

            await redis.StringSetAsync(
                cacheKey,
                JsonSerializer.Serialize(book),
                TimeSpan.FromMinutes(30)
            );

            return book;
        }

        public async Task<Book> CreateBook(CreateBookDTO bookDto)
        {
            var authors = await _dbContext.Author
                .Where(a => bookDto.AuthorIds.Contains(a.Id))
                .ToListAsync();

            var book = new Book
            {
                Title = bookDto.Title,
                Price = bookDto.Price,
                StockQuantity = bookDto.StockQuantity,
                Authors = authors
            };

            _dbContext.Books.Add(book);
            await _dbContext.SaveChangesAsync();
            return book;
        }
    }

    public class CreateBookDTO
    {
        public string Title { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public List<int> AuthorIds { get; set; } = new();
    }


}
