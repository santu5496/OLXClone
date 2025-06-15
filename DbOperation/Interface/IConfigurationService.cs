using DbOperation.Models;
using DbOperation.ViewModels;

namespace DbOperation.Interface
{
    public interface IConfigurationService

    {
        List<CarModelNamveVewModel> GetCarModels(string? search = null);
        List<CarBrands> GetCarBrands(string? search);
        CarBrands? GetCarBrandById(int id);
        bool AddCarBrand(CarBrands brand);
        bool UpdateCarBrand(CarBrands brand);
        bool DeleteCarBrand(int id);

      
        CarModels? GetCarModelById(int id);
        bool AddCarModel(CarModels model);
        bool UpdateCarModel(CarModels model);
        bool DeleteCarModel(int id);




        // CarVariants CRUD methods
        bool AddCarVariant(CarVariants variant);
        bool UpdateCarVariant(CarVariants variant);
      
        CarVariants GetCarVariantById(int id);
        bool DeleteCarVariant(int id);
        List<CarVariantViewModel> GetCarVariants(string? search = null);
        // Optional helper for filtering variants by model or brand (if needed)

        List<CarVariants> GetCarVariantsByModel(int modelId);
        List<CarModels> GetCarModelsByBrand(int brandId);



        #region start VehicleCategories CRUD

        List<VehicleCategories> GetVehicleCategories(string? search = null);
        VehicleCategories GetVehicleCategoryById(int id);
        bool AddVehicleCategory(VehicleCategories category);
        bool UpdateVehicleCategory(VehicleCategories category);
        bool DeleteVehicleCategory(int id);
        #endregion

        // FuelTypes CRUD
        List<FuelTypes> GetFuelTypes(string? search = null);

        FuelTypes? GetFuelTypeById(int id);

        bool AddFuelType(FuelTypes fuelType);

        bool UpdateFuelType(FuelTypes fuelType);

        bool DeleteFuelType(int id);



        List<TransmissionTypes> GetTransmissionTypes(string? search = null);
        TransmissionTypes GetTransmissionTypeById(int id);
        bool AddTransmissionType(TransmissionTypes transmission);
        bool UpdateTransmissionType(TransmissionTypes transmission);
        bool DeleteTransmissionType(int id);

    }

}
