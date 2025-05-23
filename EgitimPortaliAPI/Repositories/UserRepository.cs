using EgitimPortaliAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace EgitimPortaliAPI.Repositories
{
    public class UserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ApplicationUser> GetUserById(string id)
        {
            return await _context.Users
                .Include(u => u.Enrollments)
                .Include(u => u.Reviews)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<IEnumerable<ApplicationUser>> GetAllUsers()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<IEnumerable<Course>> GetUserEnrolledCourses(string userId)
        {
            return await _context.Enrollments
                .Include(e => e.Course)
                .Where(e => e.UserId == userId && e.Status == "Active")
                .Select(e => e.Course)
                .ToListAsync();
        }
    }
}