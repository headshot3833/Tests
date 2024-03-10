using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Tests.Models;

namespace Tests.Models
{
    public class EditTestModel
    {
        public int TestId { get; set; }
        public string TestTitle { get; set; }
        public List<EditQuestionModel> Questions { get; set; }
    }

    public class EditQuestionModel
    {

        public int QuestionId { get; set; }
        public string QuestionText { get; set; }
        public List<EditAnswerModel> Answers { get; set; }
        public int TestId { get; set; }

        public AnswerType QuestionType { get; set; }
    }

    public class EditAnswerModel
    {
        public int? QuestionId { get; set; }
        public int? AnswerId { get; set; }
        public string AnswerText { get; set; }

        [Required(ErrorMessage = "IsCorrect field is required")]
        public bool IsCorrect { get; set; }
        public bool IsDeleted { get; set; }

    }
}