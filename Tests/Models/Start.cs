namespace Tests.Models
{
    public class StartTestViewModel
    {
        public int TestId { get; internal set; }
        public string TestTitle { get; set; }
        public List<CreateQuestionModel> Questions { get; set; }
    }
}
