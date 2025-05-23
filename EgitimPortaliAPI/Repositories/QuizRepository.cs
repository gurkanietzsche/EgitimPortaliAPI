using EgitimPortaliAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace EgitimPortaliAPI.Repositories
{
    public class QuizRepository : GenericRepository<Quiz>
    {
        public QuizRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Quiz>> GetCourseQuizzes(int courseId)
        {
            return await _dbSet
                .Where(q => q.CourseId == courseId)
                .Include(q => q.Questions)
                    .ThenInclude(q => q.Answers)
                .ToListAsync();
        }

        public async Task<Quiz> GetQuizWithQuestions(int id)
        {
            return await _dbSet
                .Include(q => q.Questions)
                    .ThenInclude(q => q.Answers)
                .FirstOrDefaultAsync(q => q.Id == id);
        }

        public async Task<QuizResult> GetUserQuizResult(string userId, int quizId)
        {
            return await _context.QuizResults
                .FirstOrDefaultAsync(qr => qr.UserId == userId && qr.QuizId == quizId);
        }

        public async Task<IEnumerable<QuizResult>> GetQuizResults(int quizId)
        {
            return await _context.QuizResults
                .Include(qr => qr.User)
                .Where(qr => qr.QuizId == quizId)
                .ToListAsync();
        }

        // QuizResult için özel AddAsync metodu
        public async Task<QuizResult> AddQuizResultAsync(QuizResult quizResult)
        {
            _context.QuizResults.Add(quizResult);
            await _context.SaveChangesAsync();
            return quizResult;
        }
    }
}