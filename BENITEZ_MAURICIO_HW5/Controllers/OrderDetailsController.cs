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
using Microsoft.EntityFrameworkCore.Internal;


namespace BENITEZ_MAURICIO_HW5.Controllers
{
    public class OrderDetailsController : Controller
    {
        private readonly AppDbContext _context;

        public OrderDetailsController(AppDbContext context)
        {
            _context = context;
        }

        //// GET: OrderDetails
        public async Task<IActionResult> Index(int? orderID)
        {
            if (orderID == null)
            {
                return View("Error", new String[] { "Please specify an order to view!" });
            }

            //TODO: Q.12) Do I need to add a Include statment to pull product info? 
            //limit the list to only the ORDER details that belong to this ORDER
            List<OrderDetail> ods = _context.OrderDetails.Include(od => od.Product).Where(od => od.Order.OrderID == orderID).ToList();

            return View(ods);
        }
        
        

        //////GET: OrderDetails/Details/5
        //public async Task<IActionResult> Index(int? orderID)
        //{
        //    if (orderID == null)
        //    {
        //        return View("Error", new String[] { "Please specify an order to view!" });
        //    }

        //    List<OrderDetail> ods = _context.OrderDetails.Include(od => od.Product).Where(od => od.Order.OrderID == orderID).ToList();
        //    return View(ods);
        //}
        //{
        //    return View(await _context.OrderDetails.ToListAsync());
        //}

        // GET: OrderDetails/Create
        public IActionResult Create(int orderID)
        {

            //create a new instance of the ORDERDETAILS class
            OrderDetail od = new OrderDetail();

            //find the order that should be associated with this order
            Order dbOrder = _context.Orders.Find(orderID);


            //set the new order detail's order equal to the order you just found
            od.Order = dbOrder;
            //Add a variable to hold the ORDERID to create a conditional & compare it
            //Int32 newID = dbOrder.OrderID;

            //populate the ViewBag with a list of existing PRODUCTS
            ViewBag.AllProducts = GetAllProducts();

            //pass the newly created order detail to the view
            //return RedirectToAction("Details", "Orders", new { orderID = od.Order.OrderID });

            return View(od);
        }

        // POST: OrderDetails/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Order, OrderDetailID, Quantity, ProductPrice")] OrderDetail orderDetail, int SelectedProduct)
        {
            //if user has not entered all fields, send them back to try again
            if (ModelState.IsValid == false)
            {
                ViewBag.AllProducts = GetAllProducts();
                return View(orderDetail);
            }

            //find the prod to be associated with this order
            Product dbProduct = _context.Products.Find(SelectedProduct);

            //set the order detail's product to be equal to the one we just found
            orderDetail.Product = dbProduct;

            Order dbOrder = _context.Orders.Find(orderDetail.Order.OrderID);

            //set the order on the ORD detail equal to the ORDER that we just found
            orderDetail.Order = dbOrder;

            //set the ORDER detail's price equal to the producturse price this will allow us to to store the price that the user paid
            orderDetail.ProductPrice = dbProduct.ProductPrice;

            //calculate the extended price for the ORDER detail
            //TODO: FIX?
            orderDetail.ExtededPrice = orderDetail.Quantity * orderDetail.ProductPrice;

            //add the order detail to the database
            _context.Add(orderDetail);
            await _context.SaveChangesAsync();

            //send the user to the details page for this Order
            return RedirectToAction("Details", "Orders", new { id = orderDetail.Order.OrderID });
        }

        // GET: OrderDetails/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {

            //user did not specify a regiorderstration detail to edit
            if (id == null)
            {
                return View("Error", new String[] { "Please specify an order detail to edit!" });
            }

            //find the order detail
            OrderDetail orderDetail = await _context.OrderDetails.Include(od => od.Product)
                                                   .Include(od => od.Order)
                                                   .FirstOrDefaultAsync(od => od.OrderDetailID == id);
            if (orderDetail == null)
            {
                return View("Error", new String[] { "This order detail was not found" });
            }
            return View(orderDetail);
        }


        // POST: OrderDetails/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("OrderDetailID,Quantity")] OrderDetail orderDetail)
        {
            if (id != orderDetail.OrderDetailID)
            {
                return View("Error", new String[] { "There was a problem editing this record. Try again!" });
            }

            //information is not valid, try again
            if (ModelState.IsValid == false)
            {
                return View(orderDetail);
            }

            //create a new order detail
            OrderDetail dbOD;
            //if code gets this far, update the record
            try
            {
                //find the existing order detail in the database
                //include both order and PRODUUCT
                dbOD = _context.OrderDetails
                      .Include(od => od.Product)
                      .Include(od => od.Order)
                      .FirstOrDefault(od => od.OrderDetailID == orderDetail.OrderDetailID);

                //update the scalar properties
                dbOD.Quantity = orderDetail.Quantity;
                dbOD.ProductPrice = dbOD.Product.ProductPrice;
                dbOD.ExtededPrice = dbOD.Quantity * dbOD.ProductPrice;

                //save changes
                _context.Update(dbOD);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return View("Error", new String[] { "There was a problem editing this record", ex.Message });
            }

            //if code gets this far, go back to the order details index page
            return RedirectToAction("Details", "Orders", new { id = dbOD.Order.OrderID });
        }


        // GET: OrderDetails/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            //user did not specify a ORDER detail to delete
            if (id == null)
            {
                return View("Error", new String[] { "Please specify an order detail to delete!" });
            }

            //find the order detail in the database
            OrderDetail orderDetail = await _context.OrderDetails
                                                    .Include(r => r.Order)
                                                   .FirstOrDefaultAsync(m => m.OrderDetailID == id);

            //ORDER detail was not found in the database
            if (orderDetail == null)
            {
                return View("Error", new String[] { "This order detail was not in the database!" });
            }

            //send the user to the delete confirmation page
            return View(orderDetail);
        }

        // POST: OrderDetails/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            //find the regORDERistration detail to delete
            OrderDetail orderDetail = await _context.OrderDetails
                                                   .Include(r => r.Order)
                                                   .FirstOrDefaultAsync(r => r.OrderDetailID == id);

            //delete the order detail
            _context.OrderDetails.Remove(orderDetail);
            await _context.SaveChangesAsync();

            //return the user to the order/details page
            return RedirectToAction("Details", "Orders", new { id = orderDetail.Order.OrderID });
        }


        private SelectList GetAllProducts()
        {
            //create a list of the products
            List<Product> allProducts = _context.Products.ToList();

            //the user MUST select a PRODUCT, 

            //use the constructor on select list to create a new select list with the options
            SelectList slAllProducts = new SelectList(allProducts, nameof(Product.ProductID), nameof(Product.ProductName));

            return slAllProducts;
        }
    }

}


