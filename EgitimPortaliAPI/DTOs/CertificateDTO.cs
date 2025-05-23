namespace EgitimPortaliAPI.DTOs
{
    public class CertificateDto
    {
        public int Id { get; set; }
        public string CertificateNumber { get; set; }
        public DateTime IssueDate { get; set; }
        public string Url { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public int CourseId { get; set; }
        public string CourseTitle { get; set; }
    }

    public class CreateCertificateDto
    {
        public string UserId { get; set; }
        public int CourseId { get; set; }
    }
}