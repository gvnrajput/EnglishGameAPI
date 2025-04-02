using EnglishGameAPI.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Concurrent;
using System.IO;
using System.Text.Json;
using System.Threading;

namespace EnglishGameAPI.Controllers
{
    [Route("[controller]")]
    public class QuestionsController : Controller
    {
        private const string _questionsFilePath = "Models/questions.json";
        private static List<Question> _questions = new List<Question>();
        private static readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();

        // Static constructor to load data once at startup
        static QuestionsController()
        {
            LoadQuestionsFromFile();
        }

        [HttpGet("GetQuestions")]
        public IActionResult GetQuestions(QuestionType type, QuestionLevel level)
        {
            try
            {
                _lock.EnterReadLock();
                var filteredQuestions = _questions
                    .Where(q => q.Type == type && q.Level == level)
                    .OrderBy(q => q.SortOrder)
                    .Take(5)
                    .Select(q => new
                    {
                        q.Type,
                        q.QuestionId,
                        q.QuestionText,
                        q.Options,
                        q.Answer,
                        q.SortOrder
                    })
                    .ToList();

                if (filteredQuestions.Count == 0)
                {
                    return NotFound($"No questions found for Type: {type} and Level: {level}");
                }

                return Ok(filteredQuestions);
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }

        [HttpPost("AddQuestion")]
        public IActionResult AddQuestion([FromBody] Question question)
        {
            if (question == null)
            {
                return BadRequest("Invalid question data.");
            }

            _lock.EnterWriteLock();
            try
            {
                question.QuestionId = _questions.Any() ? _questions.Max(q => q.QuestionId) + 1 : 1;
                _questions.Add(question);
                SaveQuestionsToFile();
                return Ok(new { message = "Question added successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error adding question: {ex.Message}");
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        [HttpPut("UpdateQuestion/{id}")]
        public IActionResult UpdateQuestion(int id, [FromBody] Question question)
        {
            if (question == null)
            {
                return BadRequest("Invalid question data.");
            }

            _lock.EnterWriteLock();
            try
            {
                var existingQuestion = _questions.FirstOrDefault(q => q.QuestionId == id);
                if (existingQuestion == null)
                {
                    return NotFound($"Question ID {id} not found.");
                }

                existingQuestion.QuestionText = question.QuestionText;
                existingQuestion.Type = question.Type;
                existingQuestion.Level = question.Level;
                existingQuestion.Options = question.Options;
                existingQuestion.Answer = question.Answer;

                SaveQuestionsToFile();
                return Ok(new { message = "Question updated successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error updating question: {ex.Message}");
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        private static void LoadQuestionsFromFile()
        {
            if (System.IO.File.Exists(_questionsFilePath))
            {
                string jsonData = System.IO.File.ReadAllText(_questionsFilePath);
                _questions = JsonSerializer.Deserialize<List<Question>>(jsonData) ?? new List<Question>();
            }
            else
            {
                _questions = new List<Question>();
            }
        }

        private static void SaveQuestionsToFile()
        {
            string jsonData = JsonSerializer.Serialize(_questions);
            System.IO.File.WriteAllText(_questionsFilePath, jsonData);
        }
    }
}