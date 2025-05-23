using AutoMapper;
using EgitimPortaliAPI.DTOs;
using EgitimPortaliAPI.Models;
using EgitimPortaliAPI.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EgitimPortaliAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class QuizController : ControllerBase
    {
        private readonly QuizRepository _quizRepository;
        private readonly IMapper _mapper;

        public QuizController(QuizRepository quizRepository, IMapper mapper)
        {
            _quizRepository = quizRepository;
            _mapper = mapper;
        }

        [HttpGet("course/{courseId}")]
        public async Task<ActionResult<IEnumerable<QuizDto>>> GetCourseQuizzes(int courseId)
        {
            var quizzes = await _quizRepository.GetCourseQuizzes(courseId);
            return Ok(_mapper.Map<IEnumerable<QuizDto>>(quizzes));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<QuizDto>> GetQuiz(int id)
        {
            var quiz = await _quizRepository.GetQuizWithQuestions(id);
            if (quiz == null)
                return NotFound();

            return Ok(_mapper.Map<QuizDto>(quiz));
        }

        [HttpPost("submit")]
        public async Task<ActionResult<QuizResultDto>> SubmitQuiz(SubmitQuizDto submitDto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var quiz = await _quizRepository.GetQuizWithQuestions(submitDto.QuizId);

            if (quiz == null)
                return NotFound("Quiz not found");

            // skor hesaplama
            int totalScore = 0;
            foreach (var answer in submitDto.Answers)
            {
                var question = quiz.Questions.FirstOrDefault(q => q.Id == answer.QuestionId);
                if (question != null)
                {
                    var selectedAnswer = question.Answers.FirstOrDefault(a => a.Id == answer.SelectedAnswerId);
                    if (selectedAnswer != null && selectedAnswer.IsCorrect)
                    {
                        totalScore += question.Points;
                    }
                }
            }

            // quiz sonucu oluşturma
            var quizResult = new QuizResult
            {
                UserId = userId,
                QuizId = quiz.Id,
                Score = totalScore,
                CompletedDate = DateTime.Now,
                IsPassed = totalScore >= quiz.PassingScore
            };

            await _quizRepository.AddQuizResultAsync(quizResult);

            return Ok(_mapper.Map<QuizResultDto>(quizResult));
        }

        [HttpGet("result/{quizId}")]
        public async Task<ActionResult<QuizResultDto>> GetUserQuizResult(int quizId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var result = await _quizRepository.GetUserQuizResult(userId, quizId);

            if (result == null)
                return NotFound();

            return Ok(_mapper.Map<QuizResultDto>(result));
        }
    }
}