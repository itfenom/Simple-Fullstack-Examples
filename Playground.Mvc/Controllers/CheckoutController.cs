using Playground.Mvc.DAL;
using Playground.Mvc.Models;
using System;
using System.Web.Mvc;

namespace Playground.Mvc.Controllers
{
    [Authorize]
    public class CheckoutController : BaseController
    {
        private readonly ShoppingRepository _repository;

        public CheckoutController()
        {
            _repository = ShoppingRepository.GetCart(HttpContext);
        }

        [HttpGet]
        public ActionResult Index(string orderTotal)
        {
            var formattedOrderTotal = Convert.ToDecimal(orderTotal.TrimStart('$').Trim());

            var model = new OrderViewModel {Total = formattedOrderTotal};
            return View(model);
        }

        [HttpPost]
        public ActionResult Index(OrderViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Please fix error before proceeding!");
                return View(model);
            }

            var order = new Order
            {
                OrderDate = DateTime.Today.Date,
                Username = System.Web.HttpContext.Current.Session["USER_NAME"].ToString(),
                FirstName = model.FirstName,
                LastName = model.LastName,
                Address = model.Address,
                City = model.City,
                State = model.State,
                PostalCode = model.PostalCode,
                Country = model.Country,
                Email = model.Email,
                Phone = model.Phone,
                HasBeenShipped = false,
                Total = model.Total
            };

            _repository.AddOrder(order);

            var shoppingCartDetails = _repository.GetShoppingCartDetails();

            for (int i = 0; i < shoppingCartDetails.Count; i++)
            {
                var orderDetails = new OrderDetail
                {
                    OrderId = order.OrderId,
                    Username = System.Web.HttpContext.Current.Session["USER_NAME"].ToString(),
                    ProductId = shoppingCartDetails[i].ProductID,
                    Quantity = shoppingCartDetails[i].Quantity,
                    UnitPrice = shoppingCartDetails[i].Price
                };

                _repository.AddOrderDetails(orderDetails);
            }

            return RedirectToAction("ReviewOrder", new { orderID = order.OrderId });
        }

        [HttpGet]
        public ActionResult ReviewOrder(int orderId)
        {
            //Display order with user's Info for review and submit!
            var model = new ProcessOrderViewModel();
            var orderModel = _repository.GetOrderByOrderId(orderId);

            model.FirstName = orderModel.FirstName;
            model.LastName = orderModel.LastName;
            model.Address = orderModel.Address;
            model.City = orderModel.City;
            model.State = orderModel.State;
            model.PostalCode = orderModel.PostalCode;
            model.Country = orderModel.Country;
            model.Email = orderModel.Email;
            model.Total = orderModel.Total;
            model.Phone = orderModel.Phone;
            model.OrderID = orderModel.OrderId;
            model.ShoppingCartDetails = _repository.GetShoppingCartDetails();

            return View(model);
        }

        [HttpPost]
        public ActionResult ReviewOrder(ProcessOrderViewModel model)
        {
            _repository.EmptyCart();

            return View("ThankYou", new { email = model.Email });
        }

        public ActionResult ThankYou(string email)
        {
            ViewBag.Email = email;
            return View();
        }
    }
}