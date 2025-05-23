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
    public class InstructorController : ControllerBase
    {
        private readonly InstructorRepository _instructorRepository;
        private readonly IMapper _mapper;

        public InstructorController(InstructorRepository instructorRepository, IMapper mapper)
        {
            _instructorRepository = instructorRepository;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<InstructorDto>>> GetAllInstructors()
        {
            var instructors = await _instructorRepository.GetActiveInstructors();
            return Ok(_mapper.Map<IEnumerable<InstructorDto>>(instructors));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<InstructorDto>> GetInstructor(int id)
        {
            var instructor = await _instructorRepository.GetInstructorWithCourses(id);
            if (instructor == null)
                return NotFound();

            return Ok(_mapper.Map<InstructorDto>(instructor));
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<InstructorDto>> CreateInstructor(CreateInstructorDto instructorDto)
        {
            var instructor = _mapper.Map<Instructor>(instructorDto);
            var createdInstructor = await _instructorRepository.AddAsync(instructor);

            return CreatedAtAction(nameof(GetInstructor), new { id = createdInstructor.Id },
                                 _mapper.Map<InstructorDto>(createdInstructor));
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateInstructor(int id, UpdateInstructorDto instructorDto)
        {
            var existingInstructor = await _instructorRepository.GetByIdAsync(id);
            if (existingInstructor == null)
                return NotFound();

            _mapper.Map(instructorDto, existingInstructor);
            await _instructorRepository.UpdateAsync(existingInstructor);

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteInstructor(int id)
        {
            var instructor = await _instructorRepository.GetByIdAsync(id);
            if (instructor == null)
                return NotFound();

            instructor.IsActive = false;
            await _instructorRepository.UpdateAsync(instructor);

            return NoContent();
        }
    }
}