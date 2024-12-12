using BookStoreSystem.Model;
using Microsoft.EntityFrameworkCore;

namespace BookStoreSystem.Services
{
    public class AuthorService
    {
        private readonly BookStoreDbContext _context;

        public AuthorService(BookStoreDbContext context)
        {
            _context = context;
        }

        public async Task<Author> CreateAuthor(Author author)
        {
            _context.Author.Add(author);
            await _context.SaveChangesAsync();
            return author;
        }

        public async Task<List<Author>> GetAllAuthors()
        {
            return await _context.Author.Include(a => a.Books).ToListAsync();
        }

        public async Task<Author?> GetAuthor(int id)
        {
            return await _context.Author
                .Include(a => a.Books)
                .FirstOrDefaultAsync(a => a.Id == id);
        }
    }
}
