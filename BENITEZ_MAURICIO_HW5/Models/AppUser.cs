using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;


namespace BENITEZ_MAURICIO_HW5.Models
{
    public class AppUser : IdentityUser
    {
        public String FirstName { get; set; }
        public String LastName { get; set; }
        public List<Order> Orders { get; set; }

        public AppUser()
        {
            if (Orders == null)
            {
                Orders = new List<Order>();
            }
        }
    }

}
