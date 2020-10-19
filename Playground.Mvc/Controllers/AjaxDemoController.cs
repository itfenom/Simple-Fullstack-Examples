using Playground.Mvc.DAL;
using System.Web.Mvc;

namespace Playground.Mvc.Controllers
{
    public class AjaxDemoController : BaseController
    {
        private readonly IBookRepository _repository = new BookRepository();

        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetAuthors()
        {
            var authors = _repository.GetAuthors();
            return Json(authors, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetBooksForAuthor(string author)
        {
            var books = _repository.GetBooksByAuthor(author);
            return Json(books);
        }

        public PartialViewResult GetBooks()
        {
            Book[] books = _repository.GetAllBooks().ToArray();

            return PartialView(books);
        }
    }
}