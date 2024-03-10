using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MvcApp.Models;
using Tests.Models;
using System.Collections.Generic;
using System.Linq;

namespace Tests.Controllers
{
    public class TestingController : Controller
    {
        private readonly Aplicationdb _context;

        public TestingController(Aplicationdb context)
        {
            _context = context;
        }

        public IActionResult Subjects()
        {
            List<SubjectViewModel> subjects = _context.Subjects.ToList();
            return View(subjects);
        }

        public IActionResult Tests(int subjectId)
        {
            SubjectViewModel subject = _context.Subjects
                .Include(s => s.Tests)
                .FirstOrDefault(s => s.id == subjectId);

            if (subject == null)
            {
                return NotFound();
            }

            return View(subject);
        }

        public IActionResult StartTest(int testId, string fullName)
        {
            var test = _context.Tests
                .FirstOrDefault(t => t.Id == testId);

            if (test == null)
            {
                return NotFound();
            }

            var uniqueTestId = Guid.NewGuid().ToString();
            var viewModel = new StartTestViewModel
            {
                TestId = test.Id,
                TestTitle = test.Title,
            };

            ViewBag.FullName = fullName;
            ViewBag.UniqueTestId = uniqueTestId;

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult Testing(int testId, string fullName, string uniqueTestId) // Добавьте параметр uniqueTestId
        {
            var test = _context.Tests
                .Include(t => t.Questions)
                .ThenInclude(q => q.Answers)
                .FirstOrDefault(t => t.Id == testId);
            var FullName = Request.Form["fullName"];
            if (test == null)
            {
                return NotFound();
            }

            var viewModel = new StartTestViewModel
            {
                TestTitle = test.Title,
                TestId = testId,
                Questions = test.Questions.Select(q => new CreateQuestionModel
                {
                    TestId = test.Id,
                    QuestionText = q.QuestionText,
                    QuestionType = q.QuestionType,
                    Answers = q.Answers.Select(a => new CreateAnswerModel
                    {
                        AnswerId = a.Id,
                        AnswerText = a.AnswerText,
                        IsCorrect = a.IsCorrect?? false,
                        QuestionId = a.QuestionId,
                        AnswerType = a.GetAnswerType()
                    }).ToList()
                }).ToList()
            };

            ViewBag.FullName = fullName;
            ViewBag.UniqueTestId = uniqueTestId; // Передача uniqueTestId в представление

            return View(viewModel);
        }
        [HttpPost]
        public IActionResult Submit(int testId, List<List<int>> selectedAnswers, List<string> textAnswers, string fullName, string uniqueTestId)
        {
            var test = _context.Tests
                .Include(t => t.Questions)
                .ThenInclude(a => a.Answers)
                .FirstOrDefault(t => t.Id == testId);

            if (test == null)
            {
                return NotFound();
            }
            string[] textAnswersArray = textAnswers.ToArray();
            // Логика обработки выбранных и текстовых ответов
            Dictionary<int, List<int>> processedSelectedAnswers = ProcessSelectedAnswers(selectedAnswers, test.Questions);
            Dictionary<int, string> processedTextAnswers = ProcessTextAnswers(textAnswersArray); // Используйте преобразованный массив

            int correctAnswers = CalculateCorrectAnswers(processedSelectedAnswers, test.Questions);
            int incorrectAnswers = CalculateIncorrectAnswers(processedSelectedAnswers, test.Questions);

            // Преобразуйте Dictionary<int, List<int>> в Dictionary<int, int[]>
            var selectedAnswersDict = processedSelectedAnswers.ToDictionary(kv => kv.Key, kv => kv.Value.ToArray());

            var testResult = new TestResultModel
            {
                UserId = uniqueTestId,
                TestId = testId,
                FullName = fullName,
                SelectedAnswers = selectedAnswersDict, // Теперь передаем преобразованный словарь
                TextAnswers = processedTextAnswers,
                CorrectAnswers = correctAnswers,
                IncorrectAnswers = incorrectAnswers,
            };

            _context.TestResults.Add(testResult);
            _context.SaveChanges();

            ViewBag.CorrectAnswers = correctAnswers;
            ViewBag.IncorrectAnswers = incorrectAnswers;
            ViewBag.UniqueTestId = ViewBag.UniqueTestId;
            return View("SubmitResult", testResult);
        }


        private int CalculateCorrectAnswers(Dictionary<int, List<int>> processedSelectedAnswers, ICollection<QuestionModel> questions)
        {

            int correctCount = 0;

            foreach (var question in questions)
            {
                if (processedSelectedAnswers.ContainsKey(question.Id))
                {
                    var selectedIndices = processedSelectedAnswers[question.Id];

                    // Проверяем, что количество выбранных ответов совпадает с количеством правильных ответов
                    if (selectedIndices.Count == question.Answers.Count(a => a.IsCorrect == true))
                    {
                        bool allCorrect = true;

                        foreach (var selectedIndex in selectedIndices)
                        {
                            if (selectedIndex < 0 || selectedIndex >= question.Answers.Count)
                            {
                                allCorrect = false;
                                break;
                            }

                            var selectedAnswer = question.Answers.ElementAt(selectedIndex);
                            if (!(selectedAnswer.IsCorrect ?? false))
                            {
                                allCorrect = false;
                                break;
                            }
                        }

                        if (allCorrect)
                        {
                            correctCount++;
                        }
                    }
                }
            }

            return correctCount;
        }

        private Dictionary<int, List<int>> ProcessSelectedAnswers(List<List<int>> selectedAnswers, ICollection<QuestionModel> questions)
        {
            Dictionary<int, List<int>> processedAnswers = new Dictionary<int, List<int>>();

            for (int i = 0; i < selectedAnswers.Count; i++)
            {
                var question = questions.ElementAt(i);
                var selectedIndices = selectedAnswers[i]; // Список индексов выбранных ответов

                processedAnswers.Add(question.Id, selectedIndices);
            }

            return processedAnswers;
        }


        private Dictionary<int, string> ProcessTextAnswers(string[] textAnswers)
        {
            Dictionary<int, string> processedTextAnswers = new Dictionary<int, string>();

            // Логика обработки текстовых ответов
            // ...

            return processedTextAnswers;
        }
        private int CalculateIncorrectAnswers(Dictionary<int, List<int>> processedSelectedAnswers, ICollection<QuestionModel> questions)
        {
            int totalQuestions = questions.Count;
            int correctAnswers = CalculateCorrectAnswers(processedSelectedAnswers, questions);
            int incorrectAnswers = totalQuestions - correctAnswers;

            return incorrectAnswers;
        }
    }
}