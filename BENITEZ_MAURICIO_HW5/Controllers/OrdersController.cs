using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BENITEZ_MAURICIO_HW5.DAL;
using BENITEZ_MAURICIO_HW5.Models;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Authorization;

namespace BENITEZ_MAURICIO_HW5.Controllers
{
    //Only logged-in users can access Orders
    [Authorize]
    public class OrdersController : Controller
    {
        private readonly AppDbContext _context;

        public OrdersController(AppDbContext context)
        {
            _context = context;
        }

        //GET: Orders
        public IActionResult Index()
        {
            List<Order> orders;
            if (User.IsInRole("Admin"))
            {
                orders = _context.Orders.Include(o => o.OrderDetails).ToList();
            }
            else //the user is a customer, so only display their orders
            {
                orders = _context.Orders.Include(o => o.OrderDetails).Where(o => o.AppUser.UserName == User.Identity.Name).ToList();
            }
            return View(orders);
        }
        //GET: Orders
        //public async Task<IActionResult> Index()
        //{
        //    //set up a list 
        //    List<Order> Orders;
        //    if (User.IsInRole("Admin")) //this checks if it's an admin which will display all the orders
        //    {
        //        Orders = _context.Orders.Include(o => o.OrderDetails).ToList();
        //    }
        //    else //user is a customer so therefore show only his
        //    {
        //        Orders = _context.Orders.Include(s => s.OrderDetails).Where(s => s.AppUser.UserName == User.Identity.Name).ToList();
        //    }
        //    return View(Orders);

        //    //return View(await _context.Orders.ToListAsync());
        //}

        // GET: Orders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            //user didn't specify an order to view
            if (id == null)
            {
                return View("Error", new String[] { "Please specify an order to view!" });
            }
            //FIND the order in the database
            Order order = await _context.Orders
                                              .Include(r => r.OrderDetails)
                                              .ThenInclude(r => r.Product)
                                              .Include(r => r.AppUser)
                                              .FirstOrDefaultAsync(m => m.OrderID == id);

            //var order = await _context.Orders
            //    .FirstOrDefaultAsync(m => m.OrderID == id);
            // Order was not found
            if (order == null)
            {
                return View("Error", new String[] { "This order was not found!" });
            }

            //CHECK ORDER BELONGS TO USER
            if (User.IsInRole("Customer") && order.AppUser.UserName != User.Identity.Name)
            {
                return View("Error", new String[] { "This is not your order!" });
            }

            return View(order);
        }
        [Authorize(Roles = "Customer")]

        // GET: Orders/Create
        //only customers can create Orders
        [Authorize(Roles = "Customer")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Orders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Customer")]

        public async Task<IActionResult> Create([Bind("OrderID,OrderNumber,OrderDate,OrderNotes")] Order order)
        {
            //TODO: FIX THIS
            //Find the next order number from the utilities class
            order.OrderNumber = Utilities.GenerateNewOrderNumber.GetNextOrderNumber(_context);

            //Set the date of this order
            order.OrderDate = DateTime.Now;

            //Associate the orders with the logged-in customer
            order.AppUser = _context.Users.FirstOrDefault(u => u.UserName == User.Identity.Name);

            //make sure all properties are valid
            if (ModelState.IsValid == false)
            {
                return View(order);
            }

            //if code gets this far, add the order to the database
            _context.Add(order);
            await _context.SaveChangesAsync();

            return RedirectToAction("Create", "OrderDetails", new { orderID = order.OrderID });


        }

        // GET: Orders/Edit/5
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return View("Error", new String[] { "Please specify an order to edit" });
            }

            //find the ORDER in the database, and be sure to include details
            Order order = _context.Orders
                                       .Include(r => r.OrderDetails)
                                       .ThenInclude(r => r.Product)
                                       .Include(r => r.AppUser)
                                       .FirstOrDefault(r => r.OrderID == id);

            //ORDER was nout found in the database
            if (order == null)
            {
                return View("Error", new String[] { "This order was not found in the database!" });
            }

            //order does not belong to this user
            if (User.IsInRole("Customer") && order.AppUser.UserName != User.Identity.Name)
            {
                return View("Error", new String[] { "You are not authorized to edit this order!" });
            }

            //send the user to the order edit view

            return View(order);
            //_context.Add(order);
            //_context.SaveChangesAsync();
            //if (id != null)
            //{
            //    return RedirectToAction("Create", "OrderDetails", new { orderID = order.OrderID });
            //}

        }

        // POST: Orders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("OrderID,OrderNumber,OrderDate,OrderNotes")] Order order)
        {
            //this is a security measure to DOUBLE CHECK that user is editing correct order
            if (id != order.OrderID)
            {
                return View("Error", new String[] { "There was a problem editing this order. Try again!" });
            }

            //if there is something wrong with this order, try again
            if (ModelState.IsValid == false)
            {
                return View(order);
            }

            //if code gets this far, update!!
            try
            {
                //find the record in the database
                Order dbOrder = _context.Orders.Find(order.OrderID);

                //update the notes
                dbOrder.OrderNotes = order.OrderNotes;

                _context.Update(dbOrder);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return View("Error", new String[] { "There was an error updating this order!", ex.Message });
            }

            //send the user to the Orders Index page.
            return RedirectToAction(nameof(Index));
        }

    }
}
