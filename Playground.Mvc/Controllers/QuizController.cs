using Playground.Mvc.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Playground.Mvc.Controllers
{
    public class QuizController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            // ReSharper disable once JoinDeclarationAndInitializer
            IQueryable<QuestionVM> questions;

            questions = GetQuestions()
                       .Select(q => new QuestionVM
                       {
                           QuestionID = q.QuestionID,
                           QuestionText = q.QuestionText,
                           Choices = q.Choices.Select(c => new ChoiceVM
                           {
                               ChoiceID = c.ChoiceID,
                               ChoiceText = c.ChoiceText
                           }).ToList()
                       }).AsQueryable();

            return View(questions);
        }

        [HttpPost]
        public ActionResult Index(List<QuizAnswersVM> resultQuiz)
        {
            var finalResultQuiz = new List<QuizAnswersVM>();

            foreach (QuizAnswersVM answer in resultQuiz)
            {
                QuizAnswersVM result = GetAnswers().Where(a => a.QuestionID == answer.QuestionID).Select(a => new QuizAnswersVM
                {
                    QuestionID = a.QuestionID,
                    AnswerQ = a.AnswerQ,
                    // ReSharper disable once SimplifyConditionalTernaryExpression
                    isCorrect = string.IsNullOrEmpty(answer.AnswerQ) ? false : (answer.AnswerQ.ToLower().Equals(a.AnswerQ.ToLower()))
                }).FirstOrDefault();

                finalResultQuiz.Add(result);
            }

            return Json(new { result = finalResultQuiz }, JsonRequestBehavior.AllowGet);
        }

        private List<QuestionVM> GetQuestions()
        {
            var retVal = new List<QuestionVM>();

            retVal.Add(new QuestionVM { QuestionID = 101, QuestionText = "What is my name?", Choices = GetAllChoices(101) });
            retVal.Add(new QuestionVM { QuestionID = 102, QuestionText = "Which School I go to?", Choices = GetAllChoices(102) });
            retVal.Add(new QuestionVM { QuestionID = 103, QuestionText = "Color of my dad's car is?", Choices = GetAllChoices(103) });
            retVal.Add(new QuestionVM { QuestionID = 104, QuestionText = "How many brothers I have?", Choices = GetAllChoices(104) });
            retVal.Add(new QuestionVM { QuestionID = 105, QuestionText = "What is my age?", Choices = GetAllChoices(105) });

            return retVal;
        }

        private List<ChoiceVM> GetAllChoices(int questionId)
        {
            var retVal = new List<ChoiceVM>();

            switch (questionId)
            {
                case 101:
                    retVal.Add(new ChoiceVM { ChoiceID = 201, ChoiceText = "Ayyan" });
                    retVal.Add(new ChoiceVM { ChoiceID = 202, ChoiceText = "Ez'aan" });
                    retVal.Add(new ChoiceVM { ChoiceID = 203, ChoiceText = "Rayyan" });
                    retVal.Add(new ChoiceVM { ChoiceID = 204, ChoiceText = "I don't know" });
                    break;

                case 102:
                    retVal.Add(new ChoiceVM { ChoiceID = 205, ChoiceText = "Armstrong Elementry School" });
                    retVal.Add(new ChoiceVM { ChoiceID = 206, ChoiceText = "Abbet Elementry School" });
                    retVal.Add(new ChoiceVM { ChoiceID = 207, ChoiceText = "Qorvo" });
                    retVal.Add(new ChoiceVM { ChoiceID = 208, ChoiceText = "I dont't know" });
                    break;

                case 103:
                    retVal.Add(new ChoiceVM { ChoiceID = 209, ChoiceText = "Red" });
                    retVal.Add(new ChoiceVM { ChoiceID = 210, ChoiceText = "Blue" });
                    retVal.Add(new ChoiceVM { ChoiceID = 211, ChoiceText = "I don't know" });
                    retVal.Add(new ChoiceVM { ChoiceID = 212, ChoiceText = "Yellow" });
                    break;

                case 104:
                    retVal.Add(new ChoiceVM { ChoiceID = 213, ChoiceText = "I don't know" });
                    retVal.Add(new ChoiceVM { ChoiceID = 214, ChoiceText = "8" });
                    retVal.Add(new ChoiceVM { ChoiceID = 215, ChoiceText = "2" });
                    retVal.Add(new ChoiceVM { ChoiceID = 216, ChoiceText = "1" });
                    break;

                case 105:
                    retVal.Add(new ChoiceVM { ChoiceID = 213, ChoiceText = "10 years old" });
                    retVal.Add(new ChoiceVM { ChoiceID = 214, ChoiceText = "7 years old" });
                    retVal.Add(new ChoiceVM { ChoiceID = 215, ChoiceText = "I don't know" });
                    retVal.Add(new ChoiceVM { ChoiceID = 216, ChoiceText = "6 years old" });
                    break;
            }

            return retVal;
        }

        private List<QuizAnswersVM> GetAnswers()
        {
            var retVal = new List<QuizAnswersVM>();

            retVal.Add(new QuizAnswersVM { QuestionID = 101, AnswerQ = "Ez'aan" });
            retVal.Add(new QuizAnswersVM { QuestionID = 102, AnswerQ = "Abbet Elementry School" });
            retVal.Add(new QuizAnswersVM { QuestionID = 103, AnswerQ = "Red" });
            retVal.Add(new QuizAnswersVM { QuestionID = 104, AnswerQ = "1" });
            retVal.Add(new QuizAnswersVM { QuestionID = 105, AnswerQ = "7 years old" });

            return retVal;
        }
    }
}