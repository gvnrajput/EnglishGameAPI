namespace EnglishGameAPI.Models
{
    public enum QuestionType
    {
        All = 1,
        Synonym = 2,
        Antonym = 3,
        FillInTheBlanks = 4
    }

    public enum QuestionLevel
    {
        All = 1,
        Beginner = 2,
        Intermediate = 3,
        Expert = 4

    }
    public class Question
    {
        public int QuestionId { get; set; }
        public QuestionType Type { get; set; }
        public QuestionLevel Level { get; set; }
        public string QuestionText { get; set; }
        public List<string> Options { get; set; }
        public string Answer { get; set; }
    }
}
