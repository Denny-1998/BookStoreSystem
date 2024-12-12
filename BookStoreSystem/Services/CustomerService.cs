using BookStoreSystem.Model;
using Microsoft.EntityFrameworkCore;

namespace BookStoreSystem.Services
{
    public class CustomerService
    {
        private readonly BookStoreDbContext _context;

        public CustomerService(BookStoreDbContext context)
        {
            _context = context;
        }

        public async Task<Customer> CreateCustomer(CustomerDTO customerDto)
        {
            var customer = new Customer
            {
                Name = customerDto.Name,
                Email = customerDto.Email
            };

            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();
            return customer;
        }

        public async Task<List<Customer>> GetAllCustomers()
        {
            return await _context.Customers.ToListAsync();
        }

        public async Task<Customer?> GetCustomer(int id)
        {
            return await _context.Customers.FindAsync(id);
        }
    }

    public class CustomerDTO
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}
