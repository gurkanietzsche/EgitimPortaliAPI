namespace EgitimPortaliAPI.Models
{
    public class Quiz
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int PassingScore { get; set; }
        public int TimeLimit { get; set; }

        public int CourseId { get; set; }
        public virtual Course Course { get; set; }

        public virtual ICollection<Question> Questions { get; set; }
        public virtual ICollection<QuizResult> QuizResults { get; set; }
    }
}