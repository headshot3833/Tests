using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MvcApp.Models;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using Tests.Models;
using Tests.Services;

namespace Tests.Controllers
{
    [Authorize(policy: "SuperAdminPolicy")]
    public class TestCreateController : Controller
    {
        private readonly Aplicationdb _context;
        private readonly SubjectCreateService _subjectCreateService;

        public TestCreateController(Aplicationdb context, SubjectCreateService subjectCreateService)
        {
            _context = context;
            _subjectCreateService = subjectCreateService;
        }

        [HttpGet]
        public IActionResult CreateSubject()
        {
            var subjects = _subjectCreateService.GetSubjects();
            return View(subjects); // Передайте subjects в качестве модели
        }
        [HttpPost]
        public IActionResult CreateSubject(SubjectViewModel model)
        {
            if (_subjectCreateService.CreateSubject(model))
            {

                return RedirectToAction("CreateSubject");
            }

            var subjects = _subjectCreateService.GetSubjects();
            return View(subjects);
        }
        [HttpGet]
        public IActionResult Create(int subjectid)
        {
            ViewBag.SubjectViewModelid = subjectid;

            var testList = _context.Tests
                                  .Where(t => t.SubjectViewModelId == subjectid)
                                  .Include(t => t.Questions)
                                  .ToList();

            return View(testList); // Вернуть список тестов в представление
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(int subjectid, TestModel model)
        {
            var subjectViewModel = _context.Subjects.FirstOrDefault(s => s.id == subjectid);

            if (subjectViewModel != null)
            {
                model.SubjectViewModel = subjectViewModel; // Устанавливаем связь
                model.SubjectViewModelId = subjectViewModel.id; // Присваиваем внешний ключ
                if (ModelState.ContainsKey("SubjectViewModel"))
                {
                    ModelState.Remove("SubjectViewModel");
                }
                if (ModelState.IsValid)
                {
                    _context.Tests.Add(model);
                    _context.SaveChanges();
                    return RedirectToAction("Create", new { subjectid = subjectid });
                }

            }; // Вернуть список тестов, а не одну модель
            var testList = _context.Tests
                      .Where(t => t.SubjectViewModelId == subjectid)
                      .Include(t => t.Questions)
                      .ToList();

            return View("Create", testList);

        }
        [HttpGet]
        public IActionResult Edit(int testId)
        {
            var test = _context.Tests.Include(t => t.Questions).SingleOrDefault(t => t.Id == testId);

            if (test == null)
            {
                return NotFound();
            }

            var model = new TestModel
            {
                Id = test.Id,
                Title = test.Title,
                Questions = test.Questions // Передаем список вопросов в модель
            };

            return View("Edit", model); // Обновляем здесь, чтобы передать правильную модель
        }
        [HttpGet]
        public IActionResult CreateQuestion(int testId)
        {
            var testlist = _context.Tests.Include(t => t.Questions).FirstOrDefault(t => t.Id == testId);
            int testcount = testlist.Questions.Count;


            var createQuestionModel = new CreateQuestionModel
            {

                TestId = testId,
                TestCount = testcount,
                Answers = new List<CreateAnswerModel>
                {
                    new CreateAnswerModel()
                }
            };

            return View(createQuestionModel);
        }
        [HttpPost]
        public IActionResult CreateQuestion(CreateQuestionModel createQuestionModel)
        {
            if (ModelState.IsValid)
            {
                var question = new QuestionModel
                {
                    TestId = createQuestionModel.TestId,
                    QuestionText = createQuestionModel.QuestionText,
                    QuestionType = createQuestionModel.QuestionType
                };
                foreach (var answerModel in createQuestionModel.Answers)
                {
                    var answer = new AnswerModel
                    {
                        Text = answerModel.AnswerText,
                        AnswerText = answerModel.AnswerText,
                        AnswerType = answerModel.AnswerType,
                        AnswerId = answerModel.AnswerId,
                        IsCorrect = answerModel.IsCorrect
                    };

                    question.Answers.Add(answer);
                }

                _context.Questions.Add(question);
                _context.SaveChanges();
            }
            return View(createQuestionModel);
        }
        public IActionResult _CreateAnswerPartial(int answerId)
        {
            var answerModel = new CreateAnswerModel
            {
                AnswerId = answerId, // Используйте переданный answerId
                                     // Остальные поля и логика для создания модели ответа
            };
            return PartialView(answerModel);
        }
        [HttpPost]
        public IActionResult Delete(int testId, int SubjectViewModelid)
        {
            var test = _context.Tests.Include(t => t.Questions).ThenInclude(q => q.Answers).FirstOrDefault(t => t.Id == testId);

            if (test == null)
            {
                return NotFound();
            }

            // Удаление всех связанных ответов
            foreach (var question in test.Questions)
            {
                _context.Answers.RemoveRange(question.Answers);
            }

            // Удаление всех вопросов
            _context.Questions.RemoveRange(test.Questions);

            // Удаление теста
            _context.Tests.Remove(test);

            _context.SaveChanges();
            string returnUrl = HttpContext.Request.Headers["Referer"].ToString();
            return Redirect(returnUrl);
        }
        [HttpGet]
        public IActionResult EditQuestionAnswers(int testId, int questionId)
        {
            var question = _context.Questions
                .Include(q => q.Answers) // Include answers here
                .FirstOrDefault(q => q.TestId == testId && q.Id == questionId);

            if (question == null)
            {
                return NotFound();
            }

            var editQuestionModel = new EditQuestionModel
            {
                QuestionType = question.QuestionType,
                TestId = testId,
                QuestionId = question.Id,
                QuestionText = question.QuestionText,
                Answers = question.Answers.Select(answer => new EditAnswerModel
                {
                    QuestionId = question.Id,
                    AnswerId = answer.Id,
                    AnswerText = answer.AnswerText,
                    IsCorrect = answer.IsCorrect?? false
                }).ToList(),
            };

            return View("EditQuestionAnswers", editQuestionModel);
        }
        [HttpPost]
        public IActionResult DeleteAnswer(int answerId)
        {
            var answer = _context.Answers.Find(answerId);

            if (answer != null)
            {
                if (answerId < 0) // Удаляем новый ответ
                {
                    _context.Answers.Remove(answer);
                }
                else // Удаляем существующий ответ
                {
                    var question = _context.Questions.FirstOrDefault(q => q.Answers.Any(a => a.Id == answerId));
                    if (question != null)
                    {
                        question.Answers.Remove(answer);
                    }
                }

                _context.SaveChanges();

                return Json(new { success = true });
            }

            return Json(new { success = false });
        }

        [HttpPost]
        public IActionResult UpdateQuestionAnswers(EditQuestionModel editQuestionModel, int testId)
        {
            if (ModelState.IsValid)
            {

                var question = _context.Questions
                    .Include(q => q.Answers)
                    .FirstOrDefault(q => q.TestId == editQuestionModel.TestId && q.Id == editQuestionModel.QuestionId);

                if (question != null)
                {

                    question.QuestionText = editQuestionModel.QuestionText;
                    question.QuestionType = editQuestionModel.QuestionType;


                    var answerIdsInModel = editQuestionModel.Answers.Select(answer => answer.AnswerId).ToList();

                    var answersToRemove = question.Answers.Where(answer => !answerIdsInModel.Contains(answer.Id)).ToList();
                    _context.Answers.RemoveRange(answersToRemove);

                    foreach (var editAnswerModel in editQuestionModel.Answers)
                    {
                        if (editAnswerModel.AnswerId == null)
                        {

                            var newAnswer = new AnswerModel
                            {
                                Text = editAnswerModel.AnswerText,
                                AnswerText = editAnswerModel.AnswerText,
                                IsCorrect = editAnswerModel.IsCorrect,
                                QuestionId = question.Id
                            };
                            question.Answers.Add(newAnswer);
                        }
                        else
                        {

                            var answer = question.Answers.FirstOrDefault(a => a.Id == editAnswerModel.AnswerId);

                            if (answer != null)
                            {
                                answer.Text = editAnswerModel.AnswerText;
                                answer.AnswerText = editAnswerModel.AnswerText;
                                answer.IsCorrect = editAnswerModel.IsCorrect;
                            }
                        }
                    }

                    _context.SaveChanges();
                }

                return RedirectToAction("Edit", new { testId = testId });
            }

            return View("EditQuestionAnswers", editQuestionModel);
        }
        [HttpGet]
        public IActionResult GetAnswers(int testId, int questionId)
        {
            var question = _context.GetQuestionById(testId, questionId);

            if (question == null)
            {
                return NotFound();
            }

            return PartialView("_AnswersPartial", question.Answers);
        }
        [HttpPost]
        [Route("Edit/DeleteQuestion")]
        public IActionResult DeleteQuestion(int questionId, int testId)
        {
            var question = _context.Questions.Include(q => q.Answers).FirstOrDefault(q => q.Id == questionId);
            if (question == null)
            {
                return NotFound();
            }
            _context.Answers.RemoveRange(question.Answers);

            _context.Questions.Remove(question);

            _context.SaveChanges();

            return RedirectToAction("Edit", new { testId = testId });
        }
        [HttpGet]
        [Route("/Shared/_CreateAnswerPartial")]
        public IActionResult CreateAnswerPartial(AnswerType questionType, int answerIndex)
        {
            var model = new CreateAnswerModel
            {
                AnswerType = questionType
            };

            ViewData["AnswerIndex"] = answerIndex; // Устанавливаем индекс ответа в ViewData

            return PartialView("_CreateAnswerPartial", model);
        }
        [HttpPost]
        public IActionResult DeleteSubject(int subjectId)
        {
            var subject = _context.Subjects.Include(s => s.Tests).FirstOrDefault(s => s.id == subjectId);

            if (subject == null)
            {
                return NotFound();
            }

            // Удаление всех связанных тестов и их вопросов и ответов
            foreach (var test in subject.Tests)
            {
                // Удаление всех связанных ответов
                foreach (var question in test.Questions)
                {
                    _context.Answers.RemoveRange(question.Answers);
                }

                // Удаление всех вопросов
                _context.Questions.RemoveRange(test.Questions);
            }

            // Удаление всех тестов
            _context.Tests.RemoveRange(subject.Tests);

            // Удаление субъекта
            _context.Subjects.Remove(subject);

            _context.SaveChanges();

            return RedirectToAction("CreateSubject"); // Перенаправление на страницу списка субъектов
        }
    }
}