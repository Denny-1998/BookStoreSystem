﻿namespace BookStoreSystem.Model
{
    public class OrderItem
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int BookId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public Order Order { get; set; } = null!;
        public Book Book { get; set; } = null!;
    }
}