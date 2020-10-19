using System.Collections.Generic;
using System.Web.Mvc;

namespace Playground.Mvc.Controllers
{
    public class CountCharactersController : BaseController
    {
        [HttpGet]
        public ActionResult Index()
        {
            ViewBag.Output = null;
            return View();
        }

        [HttpPost]
        public ActionResult Index(string txtChars)
        {
            if (string.IsNullOrEmpty(txtChars))
            {
                return View();
            }

            var outVal = CharacterCount(txtChars);
            ViewBag.Output = outVal;
            return View();
        }

        private List<string> CharacterCount(string inputVal)
        {
            var outputString = new List<string>();
            int length = inputVal.Length;
            while (length != 0)
            {
                char character = inputVal[0];
                int count = CountChars(inputVal, character);
                inputVal = inputVal.Replace(character.ToString(), "");
                outputString.Add(character + ": appeared " + count.ToString() + " times.");
                length = length - count;
            }
            return outputString;
        }

        private int CountChars(string s, char c)
        {
            int counter = 0;
            foreach (char chr in s)
            {
                if (chr == c)
                {
                    counter++;
                }
            }
            return counter;
        }
    }
}