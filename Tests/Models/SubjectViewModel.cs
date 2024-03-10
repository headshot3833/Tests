using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tests.Models
{
    public class SubjectViewModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }

        [Required(ErrorMessage = "TitleSubject is required")]
        public string TitleSubject { get; set; }

        public ICollection<TestModel> Tests { get; set; } = new List<TestModel>();
    }
}
