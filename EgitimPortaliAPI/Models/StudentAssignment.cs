namespace EgitimPortaliAPI.Models
{
    public class StudentAssignment
    {
        public int Id { get; set; }
        public string SubmissionUrl { get; set; }
        public DateTime SubmissionDate { get; set; }
        public int Score { get; set; }
        public string Feedback { get; set; }
        public string Status { get; set; } // Submitted, Graded, Late

        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }

        public int AssignmentId { get; set; }
        public virtual Assignment Assignment { get; set; }
    }
}