using DbOperation.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbOperation.Implementation
{
    public class SoldOutCarManagementService
    {
        private readonly DbContextOptions<Assignment4Context> _context;

        public SoldOutCarManagementService(string context)
        {
            _context = new DbContextOptionsBuilder<Assignment4Context>().UseSqlServer(context).Options;
        }


        public bool DeleteCarListing(int listingId)
        {
            try
            {
                using var context = new Assignment4Context(_context);
                var listing = context.CarListings.FirstOrDefault(l => l.listingId == listingId);
                listing.listingStatus = "NonActive";
                if (listing == null) return false;
                context.CarListings.Update(listing);
                context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}