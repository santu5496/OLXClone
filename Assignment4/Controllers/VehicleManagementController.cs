using DbOperation.Interface;
using DbOperation.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace Assignment4.Controllers
{
    public class VehicleManagementController : Controller
    {
        private readonly IVehicleManagementSerivice _vehicleService;

        public VehicleManagementController(IVehicleManagementSerivice vehicleService)
        {
            _vehicleService = vehicleService;
        }

        public IActionResult VehicleManagement()
        {
            return View(); // Optional View page
        }

        // ====== CAR LISTINGS ======

        public IActionResult GetCarListings(string? search = null)
        {
            try
            {
                var data = _vehicleService.GetCarListings(search);
                return Json(data);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        public IActionResult GetCarListingById(int id)
        {
            try
            {
                var data = _vehicleService.GetCarListingById(id);
                return Json(data);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        public bool AddOrUpdateCarListing(CarListings listing)
        {
            try
            {
                if (listing.listingId == 0)
                    return _vehicleService.AddCarListing(listing);
                else
                    return _vehicleService.UpdateCarListing(listing);
            }
            catch
            {
                return false;
            }
        }

        public bool DeleteCarListing(int id)
        {
            try
            {
                return _vehicleService.DeleteCarListing(id);
            }
            catch
            {
                return false;
            }
        }

       
        // ====== LISTING FEATURES ======

        public IActionResult GetFeaturesByListingId(int listingId)
        {
            try
            {
                var data = _vehicleService.GetFeaturesByListingId(listingId);
                return Json(data);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        public bool AddOrUpdateFeature(CarListingFeatures feature)
        {
            try
            {
                if (feature.listingFeatureId == 0)
                    return _vehicleService.AddCarListingFeature(feature);
                else
                    return _vehicleService.UpdateCarListingFeature(feature);
            }
            catch
            {
                return false;
            }
        }

        public bool DeleteFeature(int listingFeatureId)
        {
            try
            {
                return _vehicleService.DeleteCarListingFeature(listingFeatureId);
            }
            catch
            {
                return false;
            }
        }

        // ====== CUSTOMER INQUIRIES ======

        public IActionResult GetInquiries(string? search = null)
        {
            try
            {
                var data = _vehicleService.GetCustomerInquiries(search);
                return Json(data);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        public IActionResult GetInquiryById(int id)
        {
            try
            {
                var data = _vehicleService.GetCustomerInquiryById(id);
                return Json(data);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        public bool AddOrUpdateInquiry(CustomerInquiries inquiry)
        {
            try
            {
                if (inquiry.inquiryId == 0)
                    return _vehicleService.AddCustomerInquiry(inquiry);
                else
                    return _vehicleService.UpdateCustomerInquiry(inquiry);
            }
            catch
            {
                return false;
            }
        }

        public bool DeleteInquiry(int id)
        {
            try
            {
                return _vehicleService.DeleteCustomerInquiry(id);
            }
            catch
            {
                return false;
            }
        }

        // ====== BUSINESS ANALYTICS ======

        public IActionResult GetAnalytics(string? search = null)
        {
            try
            {
                var data = _vehicleService.GetBusinessAnalytics(search);
                return Json(data);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        public IActionResult GetAnalyticsByListing(int listingId)
        {
            try
            {
                var data = _vehicleService.GetBusinessAnalyticsByListing(listingId);
                return Json(data);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        public bool AddOrUpdateAnalytics(BusinessAnalytics metric)
        {
            try
            {
                if (metric.analyticsId == 0)
                    return _vehicleService.AddBusinessAnalytics(metric);
                else
                    return _vehicleService.UpdateBusinessAnalytics(metric);
            }
            catch
            {
                return false;
            }
        }

        public bool DeleteAnalytics(int id)
        {
            try
            {
                return _vehicleService.DeleteBusinessAnalytics(id);
            }
            catch
            {
                return false;
            }
        }
    }
}
