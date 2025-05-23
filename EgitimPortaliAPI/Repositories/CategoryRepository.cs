using EgitimPortaliAPI.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace EgitimPortaliAPI.Repositories
{
    public class CategoryRepository
    {
        private readonly AppDbContext _context;

        public CategoryRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Category>> GetAllCategories()
        {
            var categories = await _context.Categories
                .Include(c => c.Courses)
                .Where(c => c.IsActive)
                .ToListAsync();

            Console.WriteLine(JsonConvert.SerializeObject(categories)); // Tüm veriyi logla
            return categories;
        }

        public async Task<Category> GetCategoryById(int id)
        {
            return await _context.Categories
                .Include(c => c.Courses)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Category> CreateCategory(Category category)
        {
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
            return category;
        }

        public async Task<Category> UpdateCategory(Category category)
        {
            _context.Entry(category).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return category;
        }

        public async Task<bool> DeleteCategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
                return false;

            category.IsActive = false;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}