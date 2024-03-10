using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Newtonsoft.Json;

namespace Tests.Models
{
    public class TestResultModel
    {
        [Key]
        public int Id { get; set; }

        public int TestId { get; set; } // Ссылка на тест
        public TestModel Test { get; set; }

        public string UserId { get; set; } // Идентификатор пользователя (если есть система аутентификации)
        public string FullName { get; set; }

        [NotMapped]
        public Dictionary<int, int[]> SelectedAnswers { get; set; }

        [NotMapped]
        public Dictionary<int, string> TextAnswers { get; set; }

        public int CorrectAnswers { get; set; } // Количество правильных ответов
        public int IncorrectAnswers { get; set; } // Количество неправильных ответов
    }

    public class SelectedAnswersConverter : ValueConverter<Dictionary<int, int[]>, string>
    {
        public SelectedAnswersConverter() : base(
            dict => JsonConvert.SerializeObject(dict),
            str => JsonConvert.DeserializeObject<Dictionary<int, int[]>>(str))
        { }
    }

    public class TextAnswersConverter : ValueConverter<Dictionary<int, string>, string>
    {
        public TextAnswersConverter() : base(
            dict => JsonConvert.SerializeObject(dict),
            str => JsonConvert.DeserializeObject<Dictionary<int, string>>(str))
        { }
    }
}