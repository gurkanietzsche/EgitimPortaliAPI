using EgitimPortaliAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace EgitimPortaliAPI.Repositories
{
    public class CertificateRepository : GenericRepository<Certificate>
    {
        public CertificateRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Certificate>> GetUserCertificates(string userId)
        {
            return await _dbSet
                .Include(c => c.Course)
                .Where(c => c.UserId == userId)
                .ToListAsync();
        }

        public async Task<Certificate> GetCertificateByNumber(string certificateNumber)
        {
            return await _dbSet
                .Include(c => c.User)
                .Include(c => c.Course)
                .FirstOrDefaultAsync(c => c.CertificateNumber == certificateNumber);
        }

        public async Task<bool> HasUserCertificate(string userId, int courseId)
        {
            return await _dbSet
                .AnyAsync(c => c.UserId == userId && c.CourseId == courseId);
        }
    }
}