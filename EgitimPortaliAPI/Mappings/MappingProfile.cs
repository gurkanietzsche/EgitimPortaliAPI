using AutoMapper;
using EgitimPortaliAPI.DTOs;
using EgitimPortaliAPI.Models;

namespace EgitimPortaliAPI.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Kurs Mappingleri
            CreateMap<Course, CourseDto>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name))
                .ForMember(dest => dest.InstructorName, opt => opt.MapFrom(src => $"{src.Instructor.FirstName} {src.Instructor.LastName}"))
                .ForMember(dest => dest.StudentCount, opt => opt.MapFrom(src => src.Enrollments.Count))
                .ForMember(dest => dest.AverageRating, opt => opt.MapFrom(src => src.Reviews.Any() ? src.Reviews.Average(r => r.Rating) : 0));

            CreateMap<CreateCourseDto, Course>()
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => DateTime.Now))
                .ForMember(dest => dest.UpdatedDate, opt => opt.MapFrom(src => DateTime.Now))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true));

            CreateMap<UpdateCourseDto, Course>()
                .ForMember(dest => dest.UpdatedDate, opt => opt.MapFrom(src => DateTime.Now));

            // Kullanıcı Mappingleri
            CreateMap<ApplicationUser, UserDto>();
            CreateMap<RegisterDto, ApplicationUser>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.RegistrationDate, opt => opt.MapFrom(src => DateTime.Now));

            // Kategori Mappingleri
            CreateMap<Category, CategoryDto>()
    .ForMember(dest => dest.CourseCount, opt => opt.MapFrom(src => src.Courses != null ? src.Courses.Count : 0));
            CreateMap<CreateCategoryDto, Category>()
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true));
            CreateMap<UpdateCategoryDto, Category>();

            // Ders Mappingleri
            CreateMap<Lesson, LessonDto>();
            CreateMap<CreateLessonDto, Lesson>();

            // Enrollment Mappingleri
            CreateMap<Enrollment, EnrollmentDto>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => $"{src.User.FirstName} {src.User.LastName}"))
                .ForMember(dest => dest.CourseTitle, opt => opt.MapFrom(src => src.Course.Title));
            CreateMap<CreateEnrollmentDto, Enrollment>()
                .ForMember(dest => dest.EnrollmentDate, opt => opt.MapFrom(src => DateTime.Now))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => "Active"))
                .ForMember(dest => dest.Progress, opt => opt.MapFrom(src => 0));

            // değerlendirme Mappingleri
            CreateMap<Review, ReviewDto>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => $"{src.User.FirstName} {src.User.LastName}"));
            CreateMap<CreateReviewDto, Review>()
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => DateTime.Now))
                .ForMember(dest => dest.IsApproved, opt => opt.MapFrom(src => false));

            // Instructor Mappingleri
            CreateMap<Instructor, InstructorDto>()
                .ForMember(dest => dest.CourseCount, opt => opt.MapFrom(src => src.Courses.Count));
            CreateMap<CreateInstructorDto, Instructor>()
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true));
            CreateMap<UpdateInstructorDto, Instructor>();

            // Atama mappingleri
            CreateMap<Assignment, AssignmentDto>()
                .ForMember(dest => dest.CourseTitle, opt => opt.MapFrom(src => src.Course.Title))
                .ForMember(dest => dest.SubmissionCount, opt => opt.MapFrom(src => src.StudentAssignments.Count));
            CreateMap<CreateAssignmentDto, Assignment>();

            CreateMap<StudentAssignment, StudentAssignmentDto>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => $"{src.User.FirstName} {src.User.LastName}"));

            // sertifika mappingleri
            CreateMap<Certificate, CertificateDto>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => $"{src.User.FirstName} {src.User.LastName}"))
                .ForMember(dest => dest.CourseTitle, opt => opt.MapFrom(src => src.Course.Title));

            // ödeme mappingleri
            CreateMap<Payment, PaymentDto>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => $"{src.User.FirstName} {src.User.LastName}"))
                .ForMember(dest => dest.CourseTitle, opt => opt.MapFrom(src => src.Course.Title));

            // quiz mappingleri
            CreateMap<Quiz, QuizDto>();
            CreateMap<Question, QuestionDto>();
            CreateMap<Answer, AnswerDto>();
            CreateMap<QuizResult, QuizResultDto>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => $"{src.User.FirstName} {src.User.LastName}"))
                .ForMember(dest => dest.QuizTitle, opt => opt.MapFrom(src => src.Quiz.Title));
        }
    }
}