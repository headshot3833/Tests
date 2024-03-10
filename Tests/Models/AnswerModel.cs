using System.ComponentModel.DataAnnotations;


namespace Tests.Models
{
    public class AnswerModel
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Answer text is required")]
        public string AnswerText { get; set; }
        public string Text { get; set; }

        [Display(Name = "Is Correct")]
        public bool? IsCorrect { get; set; }

        public QuestionModel Question { get; set; }
        public int QuestionId { get; set; } // Ссылка на вопрос


        // Добавим свойство AnswerId
        public int AnswerId { get; set; }

        // Добавим свойство AnswerType
        public AnswerType AnswerType { get; set; }

        // Метод для определения типа ответа
        public AnswerType GetAnswerType()
        {
            // Ваша логика для определения типа ответа
            // Например, на основе свойства AnswerType
            return AnswerType;
        }
    }
}
