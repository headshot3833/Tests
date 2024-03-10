using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tests.Models
{
    public class TestModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required(ErrorMessage = "Title is required")]
        public string Title { get; set; }

        [ForeignKey("SubjectViewModel")]
        public int SubjectViewModelId { get; set; }

        public SubjectViewModel SubjectViewModel { get; set; }
        public ICollection<QuestionModel> Questions { get; set; } = new List<QuestionModel>();
    }
}