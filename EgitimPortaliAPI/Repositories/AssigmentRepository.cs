using EgitimPortaliAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace EgitimPortaliAPI.Repositories
{
    public class AssignmentRepository : GenericRepository<Assignment>
    {
        public AssignmentRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Assignment>> GetCourseAssignments(int courseId)
        {
            return await _dbSet
                .Where(a => a.CourseId == courseId)
                .Include(a => a.StudentAssignments)
                .ToListAsync();
        }

        public async Task<Assignment> GetAssignmentWithSubmissions(int id)
        {
            return await _dbSet
                .Include(a => a.StudentAssignments)
                    .ThenInclude(sa => sa.User)
                .FirstOrDefaultAsync(a => a.Id == id);
        }
    }
}