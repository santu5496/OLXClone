

using System;
using System.Collections.Generic;

namespace DbOperation.ViewModels
{
    // Customer Search Request DTO
    public class CustomerCarSearchDto
    {
        public string searchKeyword { get; set; }
        public int? cityId { get; set; }
        public int? stateId { get; set; }

        // Basic Filters
        public int? brandId { get; set; }
        public int? modelId { get; set; }
        public int? categoryId { get; set; }
        public int? fuelTypeId { get; set; }
        public int? transmissionId { get; set; }

        // Price Range
        public decimal? minPrice { get; set; }
        public decimal? maxPrice { get; set; }

        // Year Range
        public int? minYear { get; set; }
        public int? maxYear { get; set; }

        // KM Range
        public int? minKM { get; set; }
        public int? maxKM { get; set; }

        // Quick Filters
        public bool? firstOwnerOnly { get; set; }
        public bool? verifiedOnly { get; set; }
        public bool? noAccident { get; set; }
        public bool? automaticOnly { get; set; }

        // Sorting & Pagination
        public string sortBy { get; set; } = "latest";
        public int page { get; set; } = 1;
        public int pageSize { get; set; } = 20;
    }

    // Customer Car Display DTO
    public class CustomerCarDto
    {
        public int listingId { get; set; }
        public string title { get; set; }
        public string registrationNumber { get; set; }
        // Basic Info
        public string brandName { get; set; }
        public string modelName { get; set; }
        public string variantName { get; set; }
        public int year { get; set; }
        public string fuelType { get; set; }
        public string transmission { get; set; }
        public int kilometersOnOdometer { get; set; }
        public int? totalPreviousOwners { get; set; }

        // Location
        public string cityName { get; set; }
        public string stateName { get; set; }
        public string areaOrLocality { get; set; }

        // Pricing
        public decimal sellingPrice { get; set; }
        public decimal? originalPrice { get; set; }
        public decimal? marketPrice { get; set; }
        public bool isPriceNegotiable { get; set; }

        // Condition
        public int? exteriorRating { get; set; }
        public int? interiorRating { get; set; }
        public int? engineRating { get; set; }
        public string overallCondition { get; set; }

        // Features & Highlights
        public List<string> keyFeatures { get; set; } = new List<string>();
        public List<string> highlights { get; set; } = new List<string>();

        // Status Flags
        public bool isVerified { get; set; }
        public bool isFeatured { get; set; }
        public bool isUrgentSale { get; set; }
        public bool hasAccidentHistory { get; set; }

        // Images
        public string primaryImage { get; set; }
        public List<string> additionalImages { get; set; } = new List<string>();
        public int totalImages { get; set; }

        // Seller Info
        public string sellerName { get; set; }
        public string sellerType { get; set; }
        public string sellerPhone { get; set; }

        // Analytics
        public int? totalViews { get; set; }
        public int? totalInquiries { get; set; }

        // Timestamps
        public DateTime? listingDate { get; set; }

        // Calculated Fields
        public decimal? emiAmount { get; set; }
        public decimal? savingsAmount { get; set; }
        public List<string> badges { get; set; } = new List<string>();
        public string priceLabel { get; set; } // "Great Deal", 'FirstOrDefault'"}Fair Price", etc.
    }

    // Search Results Response
    public class CustomerCarSearchResultDto
    {
        public List<CustomerCarDto> cars { get; set; } = new List<CustomerCarDto>();
        public int totalCount { get; set; }
        public int currentPage { get; set; }
        public int totalPages { get; set; }
        public bool hasNextPage { get; set; }
        public bool hasPreviousPage { get; set; }
        public string registrationNumber { get; set; }

        // Filter Options
        public List<FilterOption> availableBrands { get; set; } = new List<FilterOption>();
        public List<FilterOption> availableModels { get; set; } = new List<FilterOption>();
        public List<FilterOption> availableCategories { get; set; } = new List<FilterOption>();
        public List<FilterOption> availableFuelTypes { get; set; } = new List<FilterOption>();
        public List<FilterOption> availableTransmissions { get; set; } = new List<FilterOption>();
        public List<FilterOption> availableCities { get; set; } = new List<FilterOption>();

        // Price & Year Ranges
        public decimal minPrice { get; set; }
        public decimal maxPrice { get; set; }
        public int minYear { get; set; }
        public int maxYear { get; set; }

        // Quick Stats
        public int totalListings { get; set; }
        public int verifiedListings { get; set; }
        public int featuredListings { get; set; }
    }

    public class FilterOption
    {
        public int id { get; set; }
        public string name { get; set; }
        public int count { get; set; }
        public string registrationNumber { get; set; }
    }

    // Car Detail for Detail Page
    public class CustomerCarDetailDto : CustomerCarDto
    {
        // Extended Details
        public string detailedDescription { get; set; }
        public string sellingReason { get; set; }
        public string specialHighlights { get; set; }
        public string knownIssues { get; set; }
  

        // Complete Specifications
        public int? engineCC { get; set; }
        public int? engineBHP { get; set; }
        public decimal? mileage { get; set; }
        public int? seatingCapacity { get; set; }
        public int? bootSpace { get; set; }
        public string color { get; set; }

        // Documentation
        public string registrationNumber { get; set; }
        public string rtoCode { get; set; }
        public string insuranceType { get; set; }
        public DateTime? insuranceExpiry { get; set; }
        public DateTime? pucExpiry { get; set; }

        // Service History
        public bool hasCompleteServiceHistory { get; set; }
        public bool servicedAtAuthorized { get; set; }
        public DateTime? lastServiceDate { get; set; }
        public int? lastServiceKM { get; set; }

        // Contact & Viewing
        public bool availableForInspection { get; set; }
        public bool availableForTestDrive { get; set; }
        public string preferredContactMethod { get; set; }
        public string contactDays { get; set; }
        public string contactHours { get; set; }

        // All Features
        public List<CarFeatureGroupDto> featureGroups { get; set; } = new List<CarFeatureGroupDto>();

        // Similar Cars
        public List<CustomerCarDto> similarCars { get; set; } = new List<CustomerCarDto>();
    }

    public class CarFeatureGroupDto
    {
        public string categoryName { get; set; }
        public List<CarFeatureItemDto> features { get; set; } = new List<CarFeatureItemDto>();
    }

    public class CarFeatureItemDto
    {
        public string featureName { get; set; }
        public bool isAvailable { get; set; }
        public string condition { get; set; }
    }

    // Customer Inquiry DTO
    public class CustomerInquiryDto
    {
        public int listingId { get; set; }
        public string customerName { get; set; }
        public string customerPhone { get; set; }
        public string customerEmail { get; set; }
        public string inquiryType { get; set; }
        public string message { get; set; }
        public string budgetRange { get; set; }
        public bool financeRequired { get; set; }
    }

    // Popular Search DTO
    public class PopularSearchDto
    {
        public string searchTerm { get; set; }
        public int searchCount { get; set; }
        public string category { get; set; }
    }

    // Filter Statistics DTO


    namespace DbOperation.ViewModels
    {
        public class CarListingDto
        {
            public int listingId { get; set; }
            public string listingTitle { get; set; }
            public string registrationNumber { get; set; }
            
            public int brandId { get; set; }
            public string brandName { get; set; }

            public int modelId { get; set; }
            public string modelName { get; set; }

            public string variantName { get; set; }

            public int? categoryId { get; set; }
            public int fuelTypeId { get; set; }
            public string fuelType { get; set; }

            public int transmissionId { get; set; }
            public string transmission { get; set; }

            public int cityId { get; set; }
            public string cityName { get; set; }

            public int stateId { get; set; }
            public string stateName { get; set; }

            public string areaOrLocality { get; set; }

            public decimal sellingPriceAsked { get; set; }
            public decimal? originalPurchasePrice { get; set; }
            public decimal? currentMarketPrice { get; set; }
            public bool? isPriceNegotiable { get; set; }

            public int manufacturingYear { get; set; }
            public int kilometersOnOdometer { get; set; }

            public int? totalPreviousOwners { get; set; }

            public int? exteriorConditionRating { get; set; }
            public int? interiorConditionRating { get; set; }
            public int? engineConditionRating { get; set; }
            public string overallCondition { get; set; }

            public bool? isVerifiedListing { get; set; }
            public bool? isFeaturedListing { get; set; }
            public bool? isUrgentSale { get; set; }
            public bool? hasAccidentHistory { get; set; }

            public string sellerName { get; set; }
            public string sellerType { get; set; }
            public string sellerPrimaryPhone { get; set; }

            public int? totalViews { get; set; }
            public int? totalInquiries { get; set; }
            public DateTime listingCreatedDate { get; set; }
        }

    }

    public class FilterStatisticsDto
    {
        public Dictionary<string, int> brandCounts { get; set; } = new Dictionary<string, int>();
        public Dictionary<string, int> fuelTypeCounts { get; set; } = new Dictionary<string, int>();
        public Dictionary<string, int> transmissionCounts { get; set; } = new Dictionary<string, int>();
        public Dictionary<string, int> categoryCounts { get; set; } = new Dictionary<string, int>();
        public Dictionary<string, int> colorCounts { get; set; } = new Dictionary<string, int>();
        public Dictionary<string, int> ownershipCounts { get; set; } = new Dictionary<string, int>();
        public Dictionary<string, int> yearCounts { get; set; } = new Dictionary<string, int>();
        public Dictionary<string, int> priceRangeCounts { get; set; } = new Dictionary<string, int>();
        public Dictionary<string, int> kmRangeCounts { get; set; } = new Dictionary<string, int>();
    }

    // Home Page Data DTO
    public class HomePageDataDto
    {
        public List<CustomerCarDto> featuredCars { get; set; } = new List<CustomerCarDto>();
        public List<CustomerCarDto> recentCars { get; set; } = new List<CustomerCarDto>();
        public List<FilterOption> popularBrands { get; set; } = new List<FilterOption>();
        public List<FilterOption> popularCities { get; set; } = new List<FilterOption>();
        public List<PopularSearchDto> popularSearches { get; set; } = new List<PopularSearchDto>();
        public int totalActiveCars { get; set; }
        public int totalVerifiedCars { get; set; }
        public int totalFeaturedCars { get; set; }
        public Dictionary<string, int> carsByCategory { get; set; } = new Dictionary<string, int>();
        public Dictionary<string, int> carsByPriceRange { get; set; } = new Dictionary<string, int>();
    }
}