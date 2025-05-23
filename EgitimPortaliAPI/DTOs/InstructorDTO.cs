namespace EgitimPortaliAPI.DTOs
{
    public class InstructorDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Bio { get; set; }
        public string ProfileImageUrl { get; set; }
        public string Title { get; set; }
        public string Email { get; set; }
        public bool IsActive { get; set; }
        public int CourseCount { get; set; }
    }

    public class CreateInstructorDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Bio { get; set; }
        public string ProfileImageUrl { get; set; }
        public string Title { get; set; }
        public string Email { get; set; }
    }

    public class UpdateInstructorDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Bio { get; set; }
        public string ProfileImageUrl { get; set; }
        public string Title { get; set; }
        public string Email { get; set; }
        public bool IsActive { get; set; }
    }
}