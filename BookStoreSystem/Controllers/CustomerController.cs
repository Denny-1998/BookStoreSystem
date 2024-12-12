using BookStoreSystem.Model;
using BookStoreSystem.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookStoreSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomerController : ControllerBase
    {
        private readonly CustomerService _customerService;

        public CustomerController(CustomerService customerService)
        {
            _customerService = customerService;
        }


        [HttpPost]
        public async Task<ActionResult<Customer>> CreateCustomer(CustomerDTO customer)
        {
            var newCustomer = await _customerService.CreateCustomer(customer);
            return CreatedAtAction(nameof(GetCustomer), new { id = newCustomer.Id }, newCustomer);
        }



        [HttpGet]
        public async Task<ActionResult<List<Customer>>> GetCustomers()
        {
            var customers = await _customerService.GetAllCustomers();
            return Ok(customers);
        }



        [HttpGet("{id}")]
        public async Task<ActionResult<Customer>> GetCustomer(int id)
        {
            var customer = await _customerService.GetCustomer(id);
            if (customer == null)
                return NotFound();

            return Ok(customer);
        }
    }
}
