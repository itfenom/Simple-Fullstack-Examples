using Playground.Mvc.DAL;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Playground.Mvc.Models
{
    public class OrderViewModel
    {
        [ScaffoldColumn(false)]
        public int OrderId { get; set; }

        [ScaffoldColumn(false)]
        public System.DateTime OrderDate { get; set; }

        [ScaffoldColumn(false)]
        public string Username { get; set; }

        [Required(ErrorMessage = "First Name is required")]
        [DisplayName("First Name")]
        [StringLength(160)]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last Name is required")]
        [DisplayName("Last Name")]
        [StringLength(160)]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Address is required")]
        [StringLength(70)]
        public string Address { get; set; }

        [Required(ErrorMessage = "City is required")]
        [StringLength(40)]
        public string City { get; set; }

        [Required(ErrorMessage = "State is required")]
        [StringLength(40)]
        public string State { get; set; }

        [Required(ErrorMessage = "Postal Code is required")]
        [DisplayName("Postal Code")]
        [StringLength(10)]
        public string PostalCode { get; set; }

        [Required(ErrorMessage = "Country is required")]
        [StringLength(40)]
        public string Country { get; set; }

        [Required(ErrorMessage = "Phone is required")]
        [StringLength(24)]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Email Address is required")]
        [DisplayName("Email Address")]
        [RegularExpression(@"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}",
            ErrorMessage = "Email is is not valid.")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [ScaffoldColumn(false)]
        public decimal Total { get; set; }

        public List<OrderDetail> OrderDetails { get; set; }
    }

    public class ProcessOrderViewModel
    {
        // ReSharper disable once InconsistentNaming
        public int OrderID { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Address { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string PostalCode { get; set; }

        public string Country { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }

        public decimal Total { get; set; }

        public List<ShoppingCartDetails> ShoppingCartDetails { get; set; }
    }

    public class ShoppingCartDetailsViewModel
    {
        public IList<ShoppingCartDetails> ShoppingDetails { get; set; }
    }

    public class ShoppingCartDetails
    {
        // ReSharper disable once InconsistentNaming
        public string ItemID { get; set; }

        // ReSharper disable once InconsistentNaming
        public string CartID { get; set; }

        [Display(Name = "ID")]
        // ReSharper disable once InconsistentNaming
        public int ProductID { get; set; }

        [Display(Name = "Name")]
        public string ProductName { get; set; }

        [Display(Name = "Price (each)")]
        public double? Price { get; set; }

        [Display(Name = "Quantity")]
        public int Quantity { get; set; }

        public string DisplayPrice => $"$ {Price}";

        public string ItemTotal
        {
            get
            {
                double? result = (Price * Quantity);
                return $"$ {result}";
            }
        }
    }
}