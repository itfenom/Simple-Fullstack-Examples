using Playground.Mvc.DAL;
using Playground.Mvc.Models;
using System.Web.Mvc;

namespace Playground.Mvc.Controllers
{
    public class CartController : BaseController
    {
        private readonly ShoppingRepository _repository;

        public CartController()
        {
            _repository = ShoppingRepository.GetCart(HttpContext);
        }

        public ActionResult Index()
        {
            var model = new ShoppingCartDetailsViewModel()
            {
                ShoppingDetails = _repository.GetShoppingCartDetails()
            };

            if (model.ShoppingDetails.Count != 0)
            {
                ViewBag.OrderTotal = "$ " + _repository.GetShoppingCartTotal();
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult Index(ShoppingCartDetailsViewModel postedModel)
        {
            if (!ValidateQuantity(postedModel))
            {
                Danger("Invalid Quantity entered!", true);
                return View(postedModel);
            }

            foreach (var item in postedModel.ShoppingDetails)
            {
                _repository.UpdateCartItems(item);
            }
            return RedirectToAction("Index");
        }

        private bool ValidateQuantity(ShoppingCartDetailsViewModel postedModel)
        {
            bool retVal = true;
            foreach (var item in postedModel.ShoppingDetails)
            {
                if (item.Quantity == 0)
                {
                    retVal = false;
                    break;
                }

                bool isNumberic = int.TryParse(item.Quantity.ToString(), out _);

                if (!isNumberic)
                {
                    retVal = false;
                    break;
                }
            }
            return retVal;
        }

        public ActionResult RemoveItem(string cartId, int productId)
        {
            var p = _repository.GetProductByProductId(productId);
            _repository.RemoveItem(cartId, productId);

            Success($"<b>{"SUCCESS"}</b> Product <b>'{p.ProductName}'</b> removed from the cart successfully!", true);

            return RedirectToAction("Index");
        }

        public ActionResult EmptyCart()
        {
            _repository.EmptyCart();
            return RedirectToAction("Index");
        }
    }
}