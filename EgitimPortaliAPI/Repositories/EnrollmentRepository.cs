using EgitimPortaliAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace EgitimPortaliAPI.Repositories
{
    public class EnrollmentRepository
    {
        private readonly AppDbContext _context;

        public EnrollmentRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Enrollment>> GetUserEnrollments(string userId)
        {
            return await _context.Enrollments
                .Include(e => e.Course)
                .Include(e => e.User)
                .Where(e => e.UserId == userId)
                .ToListAsync();
        }

        public async Task<Enrollment> GetEnrollmentById(int id)
        {
            return await _context.Enrollments
                .Include(e => e.Course)
                .Include(e => e.User)
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<Enrollment> CreateEnrollment(Enrollment enrollment)
        {
            _context.Enrollments.Add(enrollment);
            await _context.SaveChangesAsync();
            return enrollment;
        }

        public async Task<bool> IsUserEnrolled(string userId, int courseId)
        {
            return await _context.Enrollments
                .AnyAsync(e => e.UserId == userId && e.CourseId == courseId);
        }

        public async Task<Enrollment> UpdateProgress(int enrollmentId, decimal progress)
        {
            var enrollment = await _context.Enrollments.FindAsync(enrollmentId);
            if (enrollment == null)
                return null;

            enrollment.Progress = progress;
            if (progress == 100)
                enrollment.Status = "Completed";

            await _context.SaveChangesAsync();
            return enrollment;
        }
    }
}