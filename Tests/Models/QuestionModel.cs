using System.ComponentModel.DataAnnotations;

namespace Tests.Models
{
    public class QuestionModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Question text is required")]
        public string QuestionText { get; set; }
        public TestModel test { get; set; }
        public int TestId { get; set; } // Ссылка на тест

        public AnswerType QuestionType { get; set; } // Тип вопроса (Text, Single, Multiple)
        public List<AnswerModel> Answers { get; set; } = new List<AnswerModel>(); // Список ответов к вопросу
    }
    public enum AnswerType
    {
        Single,
        Multiple,
        Text
    }

    public class CreateQuestionModel
    {
        public int TestId { get; set; }
        [Required(ErrorMessage = "Question text is required")]
        public string QuestionText { get; set; }
        public AnswerType QuestionType { get; set; }
        public List<CreateAnswerModel> Answers { get; set; } = new List<CreateAnswerModel>();

        public int TestCount { get; set; }
        public int AnswerId { get; set; }

    }
    public class CreateAnswerModel
    {
        [Required(ErrorMessage = "Answer text is required")]
        public string AnswerText { get; set; }
        public bool IsCorrect { get; set; } // Nullable boolean type

        public int QuestionId { get; set; }

        public AnswerType AnswerType { get; set; }

        public int AnswerId { get; set; } // Add AnswerId property
        public AnswerType GetQuestionType() // Add method to get QuestionType
        {
            return AnswerType;
        }
    }
}
