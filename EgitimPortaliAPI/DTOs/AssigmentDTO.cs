namespace EgitimPortaliAPI.DTOs
{
    public class AssignmentDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime DueDate { get; set; }
        public int MaxScore { get; set; }
        public int CourseId { get; set; }
        public string CourseTitle { get; set; }
        public int SubmissionCount { get; set; }
    }

    public class CreateAssignmentDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime DueDate { get; set; }
        public int MaxScore { get; set; }
        public int CourseId { get; set; }
    }

    public class StudentAssignmentDto
    {
        public int Id { get; set; }
        public string SubmissionUrl { get; set; }
        public DateTime SubmissionDate { get; set; }
        public int Score { get; set; }
        public string Feedback { get; set; }
        public string Status { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public int AssignmentId { get; set; }
    }

    public class SubmitAssignmentDto
    {
        public int AssignmentId { get; set; }
        public string SubmissionUrl { get; set; }
    }
}