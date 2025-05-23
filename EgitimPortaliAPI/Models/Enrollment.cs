namespace EgitimPortaliAPI.Models
{
    public class Enrollment
    {
        public int Id { get; set; }
        public DateTime EnrollmentDate { get; set; }
        public decimal PaidAmount { get; set; }
        public string Status { get; set; } // Active, Completed, Cancelled
        public decimal Progress { get; set; }

        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }

        public int CourseId { get; set; }
        public virtual Course Course { get; set; }
    }
}