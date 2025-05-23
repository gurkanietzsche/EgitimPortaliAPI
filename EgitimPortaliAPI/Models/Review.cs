using EgitimPortaliAPI.Models;

public class Review
{
    public int Id { get; set; }
    public int Rating { get; set; }
    public string Comment { get; set; }
    public DateTime CreatedDate { get; set; }
    public bool IsApproved { get; set; }

    public string UserId { get; set; }
    public virtual ApplicationUser User { get; set; }

    public int CourseId { get; set; }
    public virtual Course Course { get; set; }
}