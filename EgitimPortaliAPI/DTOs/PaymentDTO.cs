namespace EgitimPortaliAPI.DTOs
{
    public class PaymentDto
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public string PaymentMethod { get; set; }
        public string TransactionId { get; set; }
        public string Status { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public int CourseId { get; set; }
        public string CourseTitle { get; set; }
    }

    public class CreatePaymentDto
    {
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; }
        public int CourseId { get; set; }
    }
}