namespace EgitimPortaliAPI.Models
{
    public class Instructor
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Bio { get; set; }
        public string ProfileImageUrl { get; set; }
        public string Title { get; set; }
        public string Email { get; set; }
        public bool IsActive { get; set; }

        public virtual ICollection<Course> Courses { get; set; }
    }
}