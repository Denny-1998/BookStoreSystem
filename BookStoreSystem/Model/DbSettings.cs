namespace BookStoreSystem.Model
{
    public class DbSettings
    {
        public string SqlConnectionString { get; set; } = string.Empty;
        public string RedisConnectionString { get; set; } = string.Empty;
    }
}
