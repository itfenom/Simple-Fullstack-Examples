using System.Collections.Generic;
using System.Web.Mvc;

namespace Playground.Mvc.Models
{
    // ReSharper disable once UnusedMember.Global
    // ReSharper disable once InconsistentNaming
    public class QuizVM
    {
        // ReSharper disable once UnusedMember.Global
        // ReSharper disable once InconsistentNaming
        public int QuizID { get; set; }
        public string QuizName { get; set; }
        // ReSharper disable once IdentifierTypo
        public List<SelectListItem> ListOfQuizz { get; set; }
    }

    // ReSharper disable once InconsistentNaming
    public class QuestionVM
    {
        // ReSharper disable once InconsistentNaming
        public int QuestionID { get; set; }
        public string QuestionText { get; set; }
        public string QuestionType { get; set; }
        // ReSharper disable once IdentifierTypo
        public string Anwser { get; set; }
        public ICollection<ChoiceVM> Choices { get; set; }
    }

    // ReSharper disable once InconsistentNaming
    public class ChoiceVM
    {
        // ReSharper disable once InconsistentNaming
        public int ChoiceID { get; set; }
        public string ChoiceText { get; set; }
    }

    // ReSharper disable once InconsistentNaming
    public class QuizAnswersVM
    {
        // ReSharper disable once InconsistentNaming
        public int QuestionID { get; set; }
        public string QuestionText { get; set; }
        public string AnswerQ { get; set; }
        // ReSharper disable once InconsistentNaming
        public bool isCorrect { get; set; }
    }
}