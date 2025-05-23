using EgitimPortaliAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace EgitimPortaliAPI.Repositories
{
    public class InstructorRepository : GenericRepository<Instructor>
    {
        public InstructorRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Instructor>> GetActiveInstructors()
        {
            return await _dbSet
                .Where(i => i.IsActive)
                .Include(i => i.Courses)
                .ToListAsync();
        }

        public async Task<Instructor> GetInstructorWithCourses(int id)
        {
            return await _dbSet
                .Include(i => i.Courses)
                .FirstOrDefaultAsync(i => i.Id == id);
        }
    }
}