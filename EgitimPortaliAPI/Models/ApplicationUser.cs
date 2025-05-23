using Microsoft.AspNetCore.Identity;

namespace EgitimPortaliAPI.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? ProfileImageUrl { get; set; }
        public DateTime RegistrationDate { get; set; }
        public virtual ICollection<Enrollment> Enrollments { get; set; }
        public virtual ICollection<Review> Reviews { get; set; }
        public virtual ICollection<Certificate> Certificates { get; set; }
        public virtual ICollection<StudentAssignment> StudentAssignments { get; set; }
        public virtual ICollection<QuizResult> QuizResults { get; set; }
    }
}