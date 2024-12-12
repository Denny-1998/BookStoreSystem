using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;

namespace BookStoreSystem.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly BookStoreDbContext _dbContext;
        private readonly IConnectionMultiplexer _redis;



        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(BookStoreDbContext dbContext, IConnectionMultiplexer redis, ILogger<WeatherForecastController> logger)
        {
            _dbContext = dbContext;
            _redis = redis;
            _logger = logger;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }



        
        [HttpGet("test-sql")]
        public async Task<IActionResult> TestSqlConnection()
        {
            try
            {
                // Test database connection
                bool canConnect = await _dbContext.Database.CanConnectAsync();

                if (!canConnect)
                    return StatusCode(500, "Cannot connect to SQL Server");

                // Get database name
                string dbName = _dbContext.Database.GetDbConnection().Database;

                return Ok($"Successfully connected to SQL Server database: {dbName}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error testing SQL connection");
                return StatusCode(500, $"Error connecting to SQL Server: {ex.Message}");
            }
        }

        [HttpGet("test-redis")]
        public IActionResult TestRedisConnection()
        {
            try
            {
                // Test Redis connection
                var redis = _redis.GetDatabase();
                var testKey = "test:connection";
                var testValue = "Connection successful at " + DateTime.UtcNow;

                // Try to write and read a value
                redis.StringSet(testKey, testValue);
                var result = redis.StringGet(testKey);

                if (!result.HasValue)
                    return StatusCode(500, "Cannot perform operations on Redis");

                return Ok($"Successfully connected to Redis. Test value: {result}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error testing Redis connection");
                return StatusCode(500, $"Error connecting to Redis: {ex.Message}");
            }
        }
    }
}
