using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BENITEZ_MAURICIO_HW5.Models
{
    public class Supplier
    {
        public Int32 SupplierID { get; set; }

        //Supplier Name
        [Display(Name = "Supplier Name:")]
        public String SupplierName { get; set; }
        //Supplier Email
        [Display(Name = "Email:")]
        public String Email { get; set; }
        //Supplier Phone
        [Display(Name = "Phone Number:")]
        public String PhoneNumber { get; set; }

        //NAVIGATIONAL PROPERTIES
        public List<Product> Products { get; set; }

        //In order to avoid the null errors
        public Supplier()
        {
            if(Products == null)
            {
                Products = new List<Product>();
            }
        }
    }

}
