using DbOperation.Interface;
using DbOperation.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DbOperation.Implementation
{
    public class VehicleManagementService: IVehicleManagementSerivice
    {
        private readonly DbContextOptions<Assignment4Context> _dbConn;

        public VehicleManagementService(string dbConn)
        {
            _dbConn = new DbContextOptionsBuilder<Assignment4Context>()
                .UseSqlServer(dbConn)
                .Options;
        }
    

       

        // ➕ Add Car Listing
        public bool AddCarListing(CarListings listing)
        {
            try
            {
                using var context = new Assignment4Context(_dbConn);
                listing.listingCreatedDate = DateTime.Now;
                listing.listingLastModifiedDate = DateTime.Now;
                context.CarListings.Add(listing);
                context.SaveChanges();
                return true;
            }
            catch(Exception ex)
            {
                return false;
            }
        }

        // 📥 Get All Listings (with optional search on title)
        public List<CarListings> GetCarListings(string? search = null)
        {
            using var context = new Assignment4Context(_dbConn);
            var query = context.CarListings.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var lowerSearch = search.Trim().ToLower();
                query = query.Where(l => l.listingTitle.ToLower().Contains(lowerSearch));
            }

            return query.OrderByDescending(l => l.listingCreatedDate).ToList();
        }

        // 🔍 Get Listing by ID
        public CarListings? GetCarListingById(int listingId)
        {
            using var context = new Assignment4Context(_dbConn);
            return context.CarListings.FirstOrDefault(l => l.listingId == listingId);
        }

        // ✏️ Update Listing
        public bool UpdateCarListing(CarListings listing)
        {
            try
            {
                using var context = new Assignment4Context(_dbConn);
                var existing = context.CarListings.FirstOrDefault(l => l.listingId == listing.listingId);
                if (existing == null) return false;

                // Update only key fields — you can expand this as needed
                existing.listingTitle = listing.listingTitle;
                existing.brandId = listing.brandId;
                existing.modelId = listing.modelId;
                existing.variantId = listing.variantId;
                existing.kilometersOnOdometer = listing.kilometersOnOdometer;
                existing.sellingPriceAsked = listing.sellingPriceAsked;
                existing.cityId = listing.cityId;
                existing.stateId = listing.stateId;
                existing.listingStatus = listing.listingStatus;
                existing.listingLastModifiedDate = DateTime.Now;

                context.CarListings.Update(existing);
                context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        // ❌ Delete Listing
        public bool DeleteCarListing(int listingId)
        {
            try
            {
                using var context = new Assignment4Context(_dbConn);
                var listing = context.CarListings.FirstOrDefault(l => l.listingId == listingId);
                if (listing == null) return false;

                context.CarListings.Remove(listing);
                context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
        // 📸 Add Image to Listing
        public bool AddCarImage(CarImages image)
        {
            try
            {
                using var context = new Assignment4Context(_dbConn);

             

                context.CarImages.Add(image);
                context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

       
        // ➕ Add Feature to Car Listing
        public bool AddCarListingFeature(CarListingFeatures feature)
        {
            try
            {
                using var context = new Assignment4Context(_dbConn);

                feature.featureMappedDate = DateTime.Now;
                if (feature.isFeatureAvailable == null)
                    feature.isFeatureAvailable = true;

                var exists = context.CarListingFeatures
                                    .Any(f => f.listingId == feature.listingId && f.featureId == feature.featureId);
                if (exists) return false;

                context.CarListingFeatures.Add(feature);
                context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        // 📋 Get All Features for a Listing
        public List<CarListingFeatures> GetFeaturesByListingId(int listingId)
        {
            using var context = new Assignment4Context(_dbConn);
            return context.CarListingFeatures
                          .Include(f => f.feature)
                          .Where(f => f.listingId == listingId)
                          .OrderBy(f => f.featureId)
                          .ToList();
        }

        // ✏️ Update Feature on Listing
        public bool UpdateCarListingFeature(CarListingFeatures feature)
        {
            try
            {
                using var context = new Assignment4Context(_dbConn);
                var existing = context.CarListingFeatures
                                      .FirstOrDefault(f => f.listingFeatureId == feature.listingFeatureId);
                if (existing == null) return false;

                existing.isFeatureAvailable = feature.isFeatureAvailable;
                existing.featureCondition = feature.featureCondition;
                existing.featureNotes = feature.featureNotes;
                existing.featureMappedDate = DateTime.Now;

                context.CarListingFeatures.Update(existing);
                context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        // ❌ Delete Feature Mapping from Listing
        public bool DeleteCarListingFeature(int listingFeatureId)
        {
            try
            {
                using var context = new Assignment4Context(_dbConn);
                var existing = context.CarListingFeatures
                                      .FirstOrDefault(f => f.listingFeatureId == listingFeatureId);
                if (existing == null) return false;

                context.CarListingFeatures.Remove(existing);
                context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool AddCustomerInquiry(CustomerInquiries inquiry)
        {
            try
            {
                using var context = new Assignment4Context(_dbConn);
                inquiry.inquiryDate = DateTime.Now;
                inquiry.inquiryStatus ??= "New";
                inquiry.priorityLevel ??= "Medium";
                inquiry.followUpRequired ??= false;
                inquiry.purchasedCar ??= false;

                context.CustomerInquiries.Add(inquiry);
                context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public List<CustomerInquiries> GetCustomerInquiries(string? search = null)
        {
            using var context = new Assignment4Context(_dbConn);
            var query = context.CustomerInquiries
                               .Include(i => i.listing)
                               .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var lower = search.Trim().ToLower();
                query = query.Where(i =>
                    i.customerName.ToLower().Contains(lower) ||
                    (i.inquirySubject != null && i.inquirySubject.ToLower().Contains(lower))
                );
            }

            return query.OrderByDescending(i => i.inquiryDate).ToList();
        }
        public CustomerInquiries? GetCustomerInquiryById(int inquiryId)
        {
            using var context = new Assignment4Context(_dbConn);
            return context.CustomerInquiries
                          .Include(i => i.listing)
                          .FirstOrDefault(i => i.inquiryId == inquiryId);
        }
        public bool UpdateCustomerInquiry(CustomerInquiries inquiry)
        {
            try
            {
                using var context = new Assignment4Context(_dbConn);
                var existing = context.CustomerInquiries.FirstOrDefault(i => i.inquiryId == inquiry.inquiryId);
                if (existing == null) return false;

                existing.inquiryStatus = inquiry.inquiryStatus;
                existing.responseNotes = inquiry.responseNotes;
                existing.priorityLevel = inquiry.priorityLevel;
                existing.firstResponseDate = inquiry.firstResponseDate ?? existing.firstResponseDate;
                existing.lastContactDate = DateTime.Now;
                existing.followUpRequired = inquiry.followUpRequired;
                existing.purchasedCar = inquiry.purchasedCar;
                existing.finalPurchasePrice = inquiry.finalPurchasePrice;

                context.CustomerInquiries.Update(existing);
                context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool DeleteCustomerInquiry(int inquiryId)
        {
            try
            {
                using var context = new Assignment4Context(_dbConn);
                var existing = context.CustomerInquiries.FirstOrDefault(i => i.inquiryId == inquiryId);
                if (existing == null) return false;

                context.CustomerInquiries.Remove(existing);
                context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }


        #region

        public bool DeleteBusinessAnalytics(int analyticsId)
        {
            try
            {
                using var context = new Assignment4Context(_dbConn);
                var existing = context.BusinessAnalytics.FirstOrDefault(a => a.analyticsId == analyticsId);
                if (existing == null) return false;

                context.BusinessAnalytics.Remove(existing);
                context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool UpdateBusinessAnalytics(BusinessAnalytics analytics)
        {
            try
            {
                using var context = new Assignment4Context(_dbConn);
                var existing = context.BusinessAnalytics.FirstOrDefault(a => a.analyticsId == analytics.analyticsId);
                if (existing == null) return false;

                existing.metricName = analytics.metricName;
                existing.metricCategory = analytics.metricCategory;
                existing.metricValue = analytics.metricValue;
                existing.metricUnit = analytics.metricUnit;
                existing.analyticsDate = analytics.analyticsDate;
                existing.relatedListingId = analytics.relatedListingId;
                existing.calculatedDate = DateTime.Now;

                context.BusinessAnalytics.Update(existing);
                context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public List<BusinessAnalytics> GetBusinessAnalyticsByListing(int listingId)
        {
            using var context = new Assignment4Context(_dbConn);
            return context.BusinessAnalytics
                          .Where(a => a.relatedListingId == listingId)
                          .OrderByDescending(a => a.analyticsDate)
                          .ToList();
        }

        public List<BusinessAnalytics> GetBusinessAnalytics(string? search = null)
        {
            using var context = new Assignment4Context(_dbConn);
            var query = context.BusinessAnalytics.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var lower = search.Trim().ToLower();
                query = query.Where(a =>
                    a.metricName.ToLower().Contains(lower) ||
                    (a.metricCategory != null && a.metricCategory.ToLower().Contains(lower))
                );
            }

            return query.OrderByDescending(a => a.analyticsDate).ToList();
        }
        public bool AddBusinessAnalytics(BusinessAnalytics analytics)
        {
            try
            {
                using var context = new Assignment4Context(_dbConn);
                analytics.calculatedDate ??= DateTime.Now;

                context.BusinessAnalytics.Add(analytics);
                context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion



    }
}
