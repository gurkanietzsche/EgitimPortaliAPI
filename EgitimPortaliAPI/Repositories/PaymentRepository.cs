using EgitimPortaliAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace EgitimPortaliAPI.Repositories
{
    public class PaymentRepository : GenericRepository<Payment>
    {
        public PaymentRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Payment>> GetUserPayments(string userId)
        {
            return await _dbSet
                .Include(p => p.Course)
                .Where(p => p.UserId == userId)
                .OrderByDescending(p => p.PaymentDate)
                .ToListAsync();
        }

        public async Task<Payment> GetPaymentByTransactionId(string transactionId)
        {
            return await _dbSet
                .FirstOrDefaultAsync(p => p.TransactionId == transactionId);
        }

        public async Task<bool> HasUserPaidForCourse(string userId, int courseId)
        {
            return await _dbSet
                .AnyAsync(p => p.UserId == userId && p.CourseId == courseId && p.Status == "Completed");
        }
    }
}