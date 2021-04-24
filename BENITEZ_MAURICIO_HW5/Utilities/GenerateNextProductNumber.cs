using BENITEZ_MAURICIO_HW5.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BENITEZ_MAURICIO_HW5.Utilities
{
    public static class GenerateNextProductNumber
    {
        public static Int32 GetNextProductNumber(AppDbContext _context)
        {
            //set a CONST to mark where the product numbers should start
            const Int32 START_NUMBER = 100;

            Int32 intMaxProductNumber; //the current maximum prod number
            Int32 intNextProductNumber; //the product number for the next class

            if (_context.Products.Count() == 0) //there are no products in the database yet
            {
                intMaxProductNumber = START_NUMBER; //product numbers start at 101
            }
            else
            {
                intMaxProductNumber = _context.Products.Max(c => c.ProductNumber); //this is the highest number in the database right now
            }

            //You added records to the datbase before you realized 
            //that you needed this and now you have numbers less than 100 
            //in the database
            if (intMaxProductNumber < START_NUMBER)
            {
                intMaxProductNumber = START_NUMBER;
            }

            //add one to the current max to find the next one
            intNextProductNumber = intMaxProductNumber + 1;

            //return the value
            return intNextProductNumber;
        }

    }
}

