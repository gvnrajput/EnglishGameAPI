using EnglishGameAPI.Models;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Text.Json;

namespace EnglishGameAPI.Controllers
{
    [Route("[controller]")]
    public class QuestionsController : Controller
    {
        private const string _questionsFilePath = "Models/questions.json";

        [HttpGet("GetQuestions")]
        public IActionResult GetQuestions(QuestionType type, QuestionLevel level)
        {
            try
            {
                var questions = LoadQuestions();
                var filteredQuestions = questions
                    .Where(q => (type == QuestionType.All || q.Type == type) &&
                                 (level == QuestionLevel.All || q.Level == level));

                if (!filteredQuestions.Any())
                {
                    return NotFound($"No questions found for Type: {type} and Level: {level}");
                }

                // Return all matching questions
                return Ok(filteredQuestions.Select(q => new
                {
                    QuestionId = q.QuestionId,
                    QuestionText = q.QuestionText,
                    Options = q.Options,
                    Answer = q.Answer
                }));
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpPost("AddQuestion")]
        public IActionResult AddQuestion([FromBody] Question question)
        {
            if (question == null)
            {
                return BadRequest("Invalid question data provided.");
            }

            try
            {
                var questions = LoadQuestions();
                question.QuestionId = questions.Any() ? questions.Max(q => q.QuestionId) + 1 : 1; // Assign unique ID
                questions.Add(question);
                SaveQuestions(questions);

                return Ok(new { message = "Question added successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpPut("UpdateQuestion/{id}")]
        public IActionResult UpdateQuestion(int id, [FromBody] Question question)
        {
            if (question == null)
            {
                return BadRequest("Invalid question data provided.");
            }

            try
            {
                var questions = LoadQuestions();
                var existingQuestion = questions.FirstOrDefault(q => q.QuestionId == id);

                if (existingQuestion == null)
                {
                    return NotFound($"Question with ID {id} not found.");
                }

                existingQuestion.QuestionText = question.QuestionText;
                existingQuestion.Type = question.Type;
                existingQuestion.Level = question.Level;
                existingQuestion.Options = question.Options;
                existingQuestion.Answer = question.Answer;

                SaveQuestions(questions);

                return Ok(new { message = "Question updated successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        private List<Question> LoadQuestions()
        {
            if (!System.IO.File.Exists(_questionsFilePath))
            {
                return new List<Question>(); // Return empty list if file doesn't exist
            }

            string jsonData = System.IO.File.ReadAllText(_questionsFilePath);
            return JsonSerializer.Deserialize<List<Question>>(jsonData) ?? new List<Question>();
        }

        private void SaveQuestions(List<Question> questions)
        {
            string jsonData = JsonSerializer.Serialize(questions);
            System.IO.File.WriteAllText(_questionsFilePath, jsonData);
        }
    }
}