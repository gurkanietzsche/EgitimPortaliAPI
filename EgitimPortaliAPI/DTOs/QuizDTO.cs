namespace EgitimPortaliAPI.DTOs
{
    public class QuizDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int PassingScore { get; set; }
        public int TimeLimit { get; set; }
        public int CourseId { get; set; }
        public List<QuestionDto> Questions { get; set; }
    }

    public class QuestionDto
    {
        public int Id { get; set; }
        public string QuestionText { get; set; }
        public string QuestionType { get; set; }
        public int Points { get; set; }
        public List<AnswerDto> Answers { get; set; }
    }

    public class AnswerDto
    {
        public int Id { get; set; }
        public string AnswerText { get; set; }
        public bool IsCorrect { get; set; }
    }

    public class QuizResultDto
    {
        public int Id { get; set; }
        public int Score { get; set; }
        public DateTime CompletedDate { get; set; }
        public bool IsPassed { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public int QuizId { get; set; }
        public string QuizTitle { get; set; }
    }

    public class SubmitQuizDto
    {
        public int QuizId { get; set; }
        public List<QuizAnswerDto> Answers { get; set; }
    }

    public class QuizAnswerDto
    {
        public int QuestionId { get; set; }
        public int SelectedAnswerId { get; set; }
    }
}