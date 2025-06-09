using DbOperation.Models;

namespace DbOperation.Interface
{
    public interface IConfigurationService

    {
        List<CarBrands> GetCarBrands(string? search);
        CarBrands? GetCarBrandById(int id);
        bool AddCarBrand(CarBrands brand);
        bool UpdateCarBrand(CarBrands brand);
        bool DeleteCarBrand(int id);

        List<CarModels> GetCarModels(string? search = null);
        CarModels? GetCarModelById(int id);
        bool AddCarModel(CarModels model);
        bool UpdateCarModel(CarModels model);
        bool DeleteCarModel(int id);


    }

}
