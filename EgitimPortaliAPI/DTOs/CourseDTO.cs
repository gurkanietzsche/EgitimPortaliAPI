namespace EgitimPortaliAPI.DTOs
{
    public class CourseDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ShortDescription { get; set; }
        public string ImageUrl { get; set; }
        public decimal Price { get; set; }
        public int DurationInHours { get; set; }
        public string Level { get; set; }
        public string Language { get; set; }
        public bool IsActive { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public int InstructorId { get; set; }
        public string InstructorName { get; set; }
        public int StudentCount { get; set; }
        public double AverageRating { get; set; }
    }

    public class CreateCourseDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string ShortDescription { get; set; }
        public string ImageUrl { get; set; }
        public decimal Price { get; set; }
        public int DurationInHours { get; set; }
        public string Level { get; set; }
        public string Language { get; set; }
        public int CategoryId { get; set; }
        public int InstructorId { get; set; }
    }

    public class UpdateCourseDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string ShortDescription { get; set; }
        public string ImageUrl { get; set; }
        public decimal Price { get; set; }
        public int DurationInHours { get; set; }
        public string Level { get; set; }
        public string Language { get; set; }
        public bool IsActive { get; set; }
        public int CategoryId { get; set; }
        public int InstructorId { get; set; }
    }
}