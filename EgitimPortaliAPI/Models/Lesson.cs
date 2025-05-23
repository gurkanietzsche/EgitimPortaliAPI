namespace EgitimPortaliAPI.Models
{
    public class Lesson
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string VideoUrl { get; set; }
        public int DurationInMinutes { get; set; }
        public int OrderNumber { get; set; }
        public bool IsFree { get; set; }

        public int CourseId { get; set; }
        public virtual Course Course { get; set; }
    }
}