﻿using System;
using Microsoft.EntityFrameworkCore;
using BENITEZ_MAURICIO_HW5.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace BENITEZ_MAURICIO_HW5.DAL
{
    //NOTE: This class definition references the user class for this project.  
    //If your User class is called something other than AppUser, you will need
    //to change it in the line below
    public class AppDbContext: IdentityDbContext<AppUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options){ }

        //TODO: CHECK? Add Dbsets here.  Products is included as an example.  
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }



    }
}
