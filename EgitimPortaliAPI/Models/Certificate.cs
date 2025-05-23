using EgitimPortaliAPI.Models;

public class Certificate
{
    public int Id { get; set; }
    public string CertificateNumber { get; set; }
    public DateTime IssueDate { get; set; }
    public string Url { get; set; }

    public string UserId { get; set; }
    public virtual ApplicationUser User { get; set; }

    public int CourseId { get; set; }
    public virtual Course Course { get; set; }
}