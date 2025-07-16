using DbOperation.Models;
using System;
using System.Collections.Generic;

namespace DbOperation.Interface
{
    public interface IVehicleManagementSerivice
    {
        // --- Car Listings ---
        bool MarkAsSold(int listingId, string buttonName);
        bool AddCarListing(CarListings listing);
        List<CarListings> GetCarListings(string? search = null);
        CarListings? GetCarListingById(int listingId);
        bool UpdateCarListing(CarListings listing);
        bool DeleteCarListing(int listingId);

        //// --- Car Images ---
        //bool AddCarImage(CarImages image);
        //List<CarImages> GetImagesByListingId(int listingId);
        //bool UpdateCarImage(CarImages image);
        //bool DeleteCarImage(int imageId);

        // --- Car Listing Features ---
        bool AddCarListingFeature(CarListingFeatures feature);
        List<CarListingFeatures> GetFeaturesByListingId(int listingId);
        bool UpdateCarListingFeature(CarListingFeatures feature);
        bool DeleteCarListingFeature(int listingFeatureId);

        // --- Customer Inquiries ---
        bool AddCustomerInquiry(CustomerInquiries inquiry);
        List<CustomerInquiries> GetCustomerInquiries(string? search = null);
        CustomerInquiries? GetCustomerInquiryById(int inquiryId);
        bool UpdateCustomerInquiry(CustomerInquiries inquiry);
        bool DeleteCustomerInquiry(int inquiryId);

         //--- Business Analytics ---
        bool AddBusinessAnalytics(BusinessAnalytics analytics);
        List<BusinessAnalytics> GetBusinessAnalytics(string? search = null);
        List<BusinessAnalytics> GetBusinessAnalyticsByListing(int listingId);
        bool UpdateBusinessAnalytics(BusinessAnalytics analytics);
        bool DeleteBusinessAnalytics(int analyticsId);
    }
}
