namespace EgitimPortaliAPI.DTOs
{
    public class ReviewDto
    {
        public int Id { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsApproved { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public int CourseId { get; set; }
    }

    public class CreateReviewDto
    {
        public int Rating { get; set; }
        public string Comment { get; set; }
        public int CourseId { get; set; }
    }
}