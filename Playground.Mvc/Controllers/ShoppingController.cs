using Playground.Mvc.DAL;
using System.Web.Mvc;

namespace Playground.Mvc.Controllers
{
    public class ShoppingController : BaseController
    {
        private readonly ShoppingRepository _repository;

        public ShoppingController()
        {
            _repository = ShoppingRepository.GetCart(HttpContext);
        }

        public ActionResult ShoppingSelection(int categoryId)
        {
            return View(_repository.GetProductsByCategoryId(categoryId));
        }

        public ActionResult Details(int productId)
        {
            return View(_repository.GetProductByProductId(productId));
        }

        public ActionResult AddToCart(int productId)
        {
            var p = _repository.GetProductByProductId(productId);
            _repository.AddToCart(productId);
            Success($"<b>{"SUCCESS"}</b> Product <b>'{p.ProductName}'</b> added to cart successfully!", true);
            return RedirectToAction("ShoppingSelection", new { categoryID = p.CategoryID });
        }
    }
}