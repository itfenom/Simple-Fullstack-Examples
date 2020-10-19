using Playground.Mvc.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Playground.Mvc.DAL
{
    public class ShoppingRepository : IDisposable
    {
        private bool _disposed;
        private readonly SeraphEntities _context = new SeraphEntities();
        public const string CartSessionKey = "CartId";
        private string ShoppingCartId { get; set; }

        public static ShoppingRepository GetCart(HttpContextBase context)
        {
            var cart = new ShoppingRepository();
            cart.ShoppingCartId = cart.GetCartId(context);
            return cart;
        }

        public static ShoppingRepository GetCart(Controller controller)
        {
            return GetCart(controller.HttpContext);
        }

        public Product GetProductByProductId(int productId)
        {
            return (from p in _context.Products
                    where p.ProductID == productId
                    select p).FirstOrDefault();
        }

        public IEnumerable<Product> GetProductsByCategoryId(int categoryId)
        {
            var products = from p in _context.Products
                            where p.CategoryID == categoryId
                            select p;
            return products.ToList();
        }

        public void AddToCart(int id)
        {
            //Retrieve the product from the database.
            var cartItem = _context.CartItems.SingleOrDefault(
                            c => c.CartId == ShoppingCartId && c.ProductId == id);

            if (cartItem == null)
            {
                //Create a new cart item if no cart item exist!
                cartItem = new CartItem
                {
                    ItemId = Guid.NewGuid().ToString(),
                    ProductId = id,
                    CartId = ShoppingCartId,
                    Product = _context.Products.SingleOrDefault(p => p.ProductID == id),
                    Quantity = 1,
                    DateCreated = DateTime.Now
                };

                _context.CartItems.Add(cartItem);
            }
            else
            {
                cartItem.Quantity++;
            }
            _context.SaveChanges();

            UpdateCartCount();
        }

        // We're using HttpContextBase to allow access to cookies.
        public string GetCartId(HttpContextBase context)
        {
            if (HttpContext.Current.Session[CartSessionKey] == null)
            {
                if (HttpContext.Current.Session["USER_NAME"] != null)
                {
                    HttpContext.Current.Session[CartSessionKey] = HttpContext.Current.Session["USER_NAME"].ToString();
                }
                else
                {
                    // Generate a new random GUID using System.Guid class.
                    Guid tempCartId = Guid.NewGuid();
                    HttpContext.Current.Session[CartSessionKey] = tempCartId.ToString();
                }
            }

            return HttpContext.Current.Session[CartSessionKey].ToString();
        }

        public List<CartItem> GetCartItems()
        {
            var cartItems = _context.CartItems.Where(c => c.CartId == ShoppingCartId).ToList();
            return cartItems;
        }

        public int GetItemCount()
        {
            int retVal = 0;
            var query = GetCartItems();

            if (query != null)
            {
                int count = 0;
                foreach (var item in query)
                {
                    count += item.Quantity;
                }

                retVal = count;
            }
            return retVal;
        }

        public List<ShoppingCartDetails> GetShoppingCartDetails()
        {
            var query = from a in _context.CartItems
                         where a.CartId == ShoppingCartId
                         join b in _context.Products
                         on a.ProductId equals b.ProductID
                         select new ShoppingCartDetails
                         {
                             ProductID = b.ProductID,
                             ProductName = b.ProductName,
                             Price = b.UnitPrice,
                             Quantity = a.Quantity,
                             ItemID = a.ItemId,
                             CartID = a.CartId
                         };

            return query.ToList();
        }

        public void RemoveItem(string cartId, int productId)
        {
            var item = (from i in _context.CartItems
                         where i.ItemId == cartId && i.ProductId == productId
                         select i).FirstOrDefault();

            if (item != null)
            {
                _context.CartItems.Remove(item);
                _context.SaveChanges();

                UpdateCartCount();
            }
        }

        public decimal GetShoppingCartTotal()
        {
            // Multiply product price by quantity of that product to get
            // the current price for each of those products in the cart.
            // Sum all product price totals to get the cart total.
            var total = (decimal?)(from cartItems in _context.CartItems
                               where cartItems.CartId == ShoppingCartId
                               select (int?)cartItems.Quantity *
                               cartItems.Product.UnitPrice).Sum();
            return total ?? decimal.Zero;
        }

        public void UpdateCartItems(ShoppingCartDetails model)
        {
            var modelToUpdate = (from a in _context.CartItems
                                  where a.CartId == model.CartID && a.ProductId == model.ProductID
                                  select a).FirstOrDefault();

            if (modelToUpdate != null)
            {
                _context.Entry(modelToUpdate).CurrentValues.SetValues(model);
                _context.SaveChanges();

                UpdateCartCount();
            }
        }

        public void EmptyCart()
        {
            var cart = GetShoppingCartDetails();

            if (cart != null)
            {
                foreach (var item in cart)
                {
                    RemoveItem(item.ItemID, item.ProductID);
                }

                UpdateCartCount();
            }
        }

        public void AddOrder(Order order)
        {
            _context.Orders.Add(order);
            _context.SaveChanges();
        }

        public void AddOrderDetails(OrderDetail orderDetails)
        {
            _context.OrderDetails.Add(orderDetails);
            _context.SaveChanges();
        }

        public Order GetOrderByOrderId(int orderId)
        {
            var q = (from o in _context.Orders
                      where o.OrderId == orderId
                      select o).FirstOrDefault();
            return q;
        }

        private void UpdateCartCount()
        {
            HttpContext.Current.Session["ITEM_COUNT"] = GetItemCount();
        }

        public void MigrateCart(string userName)
        {
            var shoppingCart = _context.CartItems.Where(c => c.CartId == ShoppingCartId);

            foreach (CartItem item in shoppingCart)
            {
                item.CartId = userName;
            }
            _context.SaveChanges();
        }

        #region Disposing

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion Disposing
    }
}