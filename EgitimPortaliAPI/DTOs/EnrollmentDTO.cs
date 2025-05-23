namespace EgitimPortaliAPI.DTOs
{
    public class EnrollmentDto
    {
        public int Id { get; set; }
        public DateTime EnrollmentDate { get; set; }
        public decimal PaidAmount { get; set; }
        public string Status { get; set; }
        public decimal Progress { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public int CourseId { get; set; }
        public string CourseTitle { get; set; }
    }

    public class CreateEnrollmentDto
    {
        public int CourseId { get; set; }
        public string UserId { get; set; }
        public decimal PaidAmount { get; set; }
    }
}