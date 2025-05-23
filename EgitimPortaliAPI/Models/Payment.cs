using EgitimPortaliAPI.Models;

public class Payment
{
    public int Id { get; set; }
    public decimal Amount { get; set; }
    public DateTime PaymentDate { get; set; }
    public string PaymentMethod { get; set; }
    public string TransactionId { get; set; }
    public string Status { get; set; }

    public string UserId { get; set; }
    public virtual ApplicationUser User { get; set; }

    public int CourseId { get; set; }
    public virtual Course Course { get; set; }
}