using EgitimPortaliAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace EgitimPortaliAPI.Repositories
{
    public class ReviewRepository : GenericRepository<Review>
    {
        public ReviewRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Review>> GetCourseReviews(int courseId)
        {
            return await _dbSet
                .Include(r => r.User)
                .Where(r => r.CourseId == courseId && r.IsApproved)
                .OrderByDescending(r => r.CreatedDate)
                .ToListAsync();
        }

        public async Task<double> GetCourseAverageRating(int courseId)
        {
            var reviews = await _dbSet
                .Where(r => r.CourseId == courseId && r.IsApproved)
                .ToListAsync();

            return reviews.Any() ? reviews.Average(r => r.Rating) : 0;
        }

        public async Task<bool> HasUserReviewedCourse(string userId, int courseId)
        {
            return await _dbSet
                .AnyAsync(r => r.UserId == userId && r.CourseId == courseId);
        }
    }
}