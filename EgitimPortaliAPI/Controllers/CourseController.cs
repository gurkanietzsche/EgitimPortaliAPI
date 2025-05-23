using AutoMapper;
using EgitimPortaliAPI.DTOs;
using EgitimPortaliAPI.Models;
using EgitimPortaliAPI.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EgitimPortaliAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourseController : ControllerBase
    {
        private readonly CourseRepository _courseRepository;
        private readonly IMapper _mapper;

        public CourseController(CourseRepository courseRepository, IMapper mapper)
        {
            _courseRepository = courseRepository;
            _mapper = mapper;
        }

        // kursları listeleme metodu

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CourseDto>>> GetAllCourses()
        {
            var courses = await _courseRepository.GetAllCourses();
            return Ok(_mapper.Map<IEnumerable<CourseDto>>(courses));
        }

        //kursları id ye göre listeleme metodu

        [HttpGet("{id}")]
        public async Task<ActionResult<CourseDto>> GetCourse(int id)
        {
            var course = await _courseRepository.GetCourseById(id);
            if (course == null)
                return NotFound();

            return Ok(_mapper.Map<CourseDto>(course));
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Instructor")]
        public async Task<ActionResult<CourseDto>> CreateCourse(CreateCourseDto courseDto)
        {
            var course = _mapper.Map<Course>(courseDto);
            var createdCourse = await _courseRepository.CreateCourse(course);

            return CreatedAtAction(nameof(GetCourse), new { id = createdCourse.Id },
                                 _mapper.Map<CourseDto>(createdCourse));
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Instructor")]
        public async Task<IActionResult> UpdateCourse(int id, UpdateCourseDto courseDto)
        {
            var existingCourse = await _courseRepository.GetCourseById(id);
            if (existingCourse == null)
                return NotFound();

            _mapper.Map(courseDto, existingCourse);
            await _courseRepository.UpdateCourse(existingCourse);

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteCourse(int id)
        {
            var result = await _courseRepository.DeleteCourse(id);
            if (!result)
                return NotFound();

            return NoContent();
        }

        [HttpGet("category/{categoryId}")]
        public async Task<ActionResult<IEnumerable<CourseDto>>> GetCoursesByCategory(int categoryId)
        {
            var courses = await _courseRepository.GetCoursesByCategory(categoryId);
            return Ok(_mapper.Map<IEnumerable<CourseDto>>(courses));
        }

        [HttpGet("instructor/{instructorId}")]
        public async Task<ActionResult<IEnumerable<CourseDto>>> GetCoursesByInstructor(int instructorId)
        {
            var courses = await _courseRepository.GetCoursesByInstructor(instructorId);
            return Ok(_mapper.Map<IEnumerable<CourseDto>>(courses));
        }
    }
}