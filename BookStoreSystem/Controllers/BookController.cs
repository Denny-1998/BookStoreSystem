using BookStoreSystem.Model;
using BookStoreSystem.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookStoreSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookController : ControllerBase
    {
        private readonly BookService _bookService;

        public BookController(BookService bookService)
        {
            _bookService = bookService;
        }

        [HttpGet]
        public async Task<ActionResult<List<Book>>> GetBooks()
        {
            var books = await _bookService.GetAllBooks();
            return Ok(books);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Book>> GetBook(int id)
        {
            var book = await _bookService.GetBook(id);
            if (book == null)
                return NotFound();

            return Ok(book);
        }

        [HttpPost]
        public async Task<ActionResult<Book>> CreateBook(CreateBookDTO book)
        {
            var newBook = await _bookService.CreateBook(book);
            return CreatedAtAction(nameof(GetBook), new { id = newBook.Id }, newBook);
        }
    }
}
