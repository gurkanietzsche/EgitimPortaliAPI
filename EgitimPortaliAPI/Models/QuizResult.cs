namespace EgitimPortaliAPI.Models
{
    public class QuizResult
    {
        public int Id { get; set; }
        public int Score { get; set; }
        public DateTime CompletedDate { get; set; }
        public bool IsPassed { get; set; }

        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }

        public int QuizId { get; set; }
        public virtual Quiz Quiz { get; set; }
    }
}