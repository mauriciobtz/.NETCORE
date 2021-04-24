using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BENITEZ_MAURICIO_HW5.Models
{
    public class Order
    {
        //CONSTANT
        private const Decimal TAX_RATE = 0.0825m;
        public Int32 OrderID { get; set; }

        //Order Number
        [Display(Name = "Order Number:")]
        public Int32 OrderNumber { get; set; }

        //order date
        [Display(Name = "Order Date:")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime OrderDate { get; set; }

        //order notes
        [Display(Name = "Order Notes:")]
        public String OrderNotes { get; set; }

        //read only properties

        [Display(Name = "Order Subtotal")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public Decimal OrderSubtotal
        {
            get { return OrderDetails.Sum(rd => rd.ExtededPrice); }
        }

        [Display(Name = "Sales Tax")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public Decimal SalesTax
        {
            get { return OrderSubtotal * TAX_RATE; }
        }

        [Display(Name = "Order Total")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public Decimal OrderTotal
        {
            get { return OrderSubtotal + SalesTax; }
        }

        // Navigational Properties
        public List<OrderDetail> OrderDetails { get; set; }

                 //Connect the user to the Order
        public AppUser AppUser { get; set; }

        //Handle Null
        public Order()
        {
            if (OrderDetails == null)
            {
                OrderDetails = new List<OrderDetail>();
            }
        }
    }
}
