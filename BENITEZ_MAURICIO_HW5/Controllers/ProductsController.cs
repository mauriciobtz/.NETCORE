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
    //Only adminds can edit access this
    [Authorize(Roles = "Admin")]
    public class ProductsController : Controller
    {
        private readonly AppDbContext _context;

        public ProductsController(AppDbContext context)
        {
            _context = context;
        }
        [AllowAnonymous]
        // GET: Products
        public async Task<IActionResult> Index()
        {
            return View(await _context.Products.ToListAsync());
        }
        [AllowAnonymous]
        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            //id was not specified - show the user an error
            if (id == null)
            {
                return View("Error", new String[] { "Please specify a product to view!" });
            }

    
            Product product = await _context.Products
                .Include(c => c.Suppliers)
                .FirstOrDefaultAsync(m => m.ProductID == id);

            //product was not found in the database
            if (product == null)
            {
                return View("Error", new String[] { "That product was not found in the database." });
            }

            return View(product);
        }

        // GET: Products/Create
        //[Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            //populate the viewbag
            ViewBag.AllSuppliers = GetAllSuppliers();
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProductID,ProductName,ProductDescription,ProductPrice,ProductType")] Product product, int[] SelectedSuppliers)
        {
            if (ModelState.IsValid==false)
            {
                //Re-populate the viewbag again
                ViewBag.AllSuppliers = GetAllSuppliers();

                //go back
                return View(product);
            }
            _context.Add(product);
            await _context.SaveChangesAsync();

            foreach (int supplierID in SelectedSuppliers)
            {
                //find supplier associated with that ID
                Supplier dbSupplier = _context.Suppliers.Find(supplierID);
                //add the suplier to the list of suppliers and save changes
                product.Suppliers.Add(dbSupplier);
                _context.SaveChanges();
            }
            //return View(product);
            return RedirectToAction(nameof(Index));
        }

        // GET: Products/Edit/5
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return View("Error", new string[] { "Please specify the product to edit" });
            }

            Product product = await _context.Products.Include(p => p.Suppliers).FirstOrDefaultAsync(predicate => predicate.ProductID == id);
            //var product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                return View("Error", new string[] { "This product was not found" });
            }
            ViewBag.AllSuppliers = GetAllSuppliers(product);
            return View(product);

        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, [Bind("ProductID,ProductName,ProductDescription,ProductPrice,ProductType")] Product product, int[] SelectedSuppliers)
        {
            if (id != product.ProductID)
            {
                return View("Error", new string[] { "Please try again!" });
            }

            if (ModelState.IsValid == false) //there is an error?
            {

                ViewBag.AllSuppliers = GetAllSuppliers(product);
                return View(product);
            }

            //if code gets this far, attempt to edit the product
            try
            {
                //Find the product to edit in the database and include relevant 
                //navigational properties
                Product dbProduct = _context.Products
                    .Include(c => c.Suppliers)
                    .FirstOrDefault(c => c.ProductID == product.ProductID);

                //create a list of supplier that need to be removed
                List<Supplier> SuppliersToRemove = new List<Supplier>();


                foreach (Supplier supplier in dbProduct.Suppliers)
                {
                    //see if the new list contains the supplier id from the old list
                    if (SelectedSuppliers.Contains(supplier.SupplierID) == false)//this supplier is not on the new list
                    {
                        SuppliersToRemove.Add(supplier);
                    }
                }

                //remove the supplier you found in the list above
                //this has to be 2 separate steps because you can't iterate (loop)
                //over a list that you are removing things from
                foreach (Supplier supplier in SuppliersToRemove)
                {
                    //remove this product supplier from the product's list of supplier
                    dbProduct.Suppliers.Remove(supplier);
                    _context.SaveChanges();
                }

                //add the suppliers to existing
                foreach (int supplierID in SelectedSuppliers)
                {
                    if (dbProduct.Suppliers.Any(d => d.SupplierID == supplierID) == false)//this supplier isn't  associated with this product
                    {
                        //Find the associated supplier in the DB
                        Supplier dbSupplier = _context.Suppliers.Find(supplierID);

                        //Add the supplier to the product's list of suppliers
                        dbProduct.Suppliers.Add(dbSupplier);
                        _context.SaveChanges();
                    }
                }

                //update the PRODUCT's scalar properties
                dbProduct.ProductName = product.ProductName;
                dbProduct.ProductDescription = product.ProductDescription;
                dbProduct.ProductPrice = product.ProductPrice;
                dbProduct.ProductType = product.ProductType;



                //save the changes
                _context.Products.Update(dbProduct);
                _context.SaveChanges();

            }
            catch (Exception ex)
            {
                return View("Error", new string[] { "There was an error editing this product.", ex.Message });
            }

            //if code gets this far, everything is okay
            //send the user back to the page with all the product
            return RedirectToAction(nameof(Index));
        }



        // THIS IS NOT NEEDED SINCE NO ONE SHOULD DELETE A PRODUCT since it may lead to orphaned children
        //    // GET: Products/Delete/5
        //    public async Task<IActionResult> Delete(int? id)
        //    {
        //        if (id == null)
        //        {
        //            return NotFound();
        //        }

        //        var product = await _context.Products
        //            .FirstOrDefaultAsync(m => m.ProductID == id);
        //        if (product == null)
        //        {
        //            return NotFound();
        //        }

        //        return View(product);
        //    }

        //    // POST: Products/Delete/5
        //    [HttpPost, ActionName("Delete")]
        //    [ValidateAntiForgeryToken]
        //    public async Task<IActionResult> DeleteConfirmed(int id)
        //    {
        //        var product = await _context.Products.FindAsync(id);
        //        _context.Products.Remove(product);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.ProductID == id);
        }

        private MultiSelectList GetAllSuppliers()
        {
            //create a new list
            List<Supplier> allSuppliers = _context.Suppliers.ToList();

            //Use multiselect constructor
            MultiSelectList mslAllSuppliers = new MultiSelectList(allSuppliers.OrderBy(d => d.SupplierName), "SupplierID", "SupplierName");

            //return
            return mslAllSuppliers;

        }

        private MultiSelectList GetAllSuppliers(Product product)
        {
            //New list of suppliers
            List<Supplier> allSuppliers = _context.Suppliers.ToList();

            //Get all the product suppliers by iterating through
            List<Int32> selectedSupplierIDs = new List<Int32>();

            foreach (Supplier associatedSupplier in product.Suppliers)
            {
                selectedSupplierIDs.Add(associatedSupplier.SupplierID);
            }

            MultiSelectList mslAllSuppliers = new MultiSelectList(allSuppliers.OrderBy(d => d.SupplierName), "SupplierID", "SupplierName", selectedSupplierIDs);

            return mslAllSuppliers;
        }
    }
}
