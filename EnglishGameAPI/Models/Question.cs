namespace EnglishGameAPI.Models
{
    public enum QuestionType
    {
        Synonym = 1,
        Antonym = 2,
        FillInTheBlanks = 3,
        Idioms = 4
    }

    public enum QuestionLevel
    {
        Level1 = 1,
        Level2 = 2,
        Level3 = 3,
        Level4 = 4,
        Level5 = 5
    }
    public class Question
    {
        public int QuestionId { get; set; }
        public QuestionType Type { get; set; }
        public QuestionLevel Level { get; set; }
        public string QuestionText { get; set; }
        public List<string> Options { get; set; }
        public string Answer { get; set; }
        public int SortOrder { get; set; }
    }
}
