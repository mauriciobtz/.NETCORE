using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BENITEZ_MAURICIO_HW5.Models
{
    public class OrderDetail
    {

        [Display(Name = "OrderDetailID")]

        public Int32 OrderDetailID { get; set; }

        // Detail Quantity
        [Display(Name = "Quantity")]
        [Range(1, 1000, ErrorMessage = "Number of products must be between 1 and 1000")]
        public Int32 Quantity { get; set; }

        //Price total
        [Display(Name = "Price")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public Decimal ProductPrice { get; set; }

        //Extended order

        [Display(Name = "Extended Price")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public Decimal ExtededPrice { get; set; }




        // Navigational Properties
        public Order Order { get; set; }
        public Product Product { get; set; }

    }
}
