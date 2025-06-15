using DbOperation.Models;
using System.Collections.Generic;
using static DbOperation.Implementation.ConfigurationService1;

namespace DbOperation.Interface
{
    public interface IConfigurationService1
    {
        // Car Colors
        List<CarColors> GetAllColours();
        CarColors AddCarColor(CarColors color);
        bool UpdateCarColor(CarColors color);
        bool DeleteCarColor(int id);

        // Car Condition Levels
        List<CarConditionLevels> GetAllCarConditions();
        CarConditionLevels GetCarConditionById(int id);
        bool AddCarCondition(CarConditionLevels condition);
        bool UpdateCarCondition(CarConditionLevels condition);
        bool DeleteCarCondition(int id);

        // Geographic States
        List<GeographicStates> GetAllStates();
        GeographicStates GetStateById(int id);
        bool AddState(GeographicStates state);
        bool UpdateState(GeographicStates state);
        bool DeleteState(int id);

        // Geographic Cities
        List<CityDto> GetAllCities();
        List<GeographicCities> GetCitiesByState(int stateId);
        GeographicCities GetCityById(int id);
        GeographicCities AddCity(GeographicCities city);
        GeographicCities UpdateCity(GeographicCities city);
        bool DeleteCity(int id);

        // Engine Specifications
        List<EngineSpecifications> GetAllEngineSpecs();
        EngineSpecifications GetEngineSpecById(int id);
        EngineSpecifications AddEngineSpec(EngineSpecifications engineSpec);
        EngineSpecifications UpdateEngineSpec(EngineSpecifications engineSpec);
        bool DeleteEngineSpec(int id);

        // Car Features
        List<CarFeaturesList> GetAllCarFeatures();
        List<CarFeaturesList> GetFeaturesByCategory(string category);
        CarFeaturesList GetCarFeatureById(int id);
        CarFeaturesList AddCarFeature(CarFeaturesList feature);
        CarFeaturesList UpdateCarFeature(CarFeaturesList feature);
        bool DeleteCarFeature(int id);

        // Insurance Providers
        List<InsuranceProviders> GetAllInsuranceProviders();
        InsuranceProviders GetInsuranceProviderById(int id);
        InsuranceProviders AddInsuranceProvider(InsuranceProviders provider);
        InsuranceProviders UpdateInsuranceProvider(InsuranceProviders provider);
        bool DeleteInsuranceProvider(int id);

        // RTO Codes
        List<RTOCodes> GetAllRTOCodes();
        List<RTOCodes> GetRTOCodesByState(int stateId);
        RTOCodes GetRTOCodeById(int id);
        RTOCodes GetRTOCodeByCode(string code);
        RTOCodes AddRTOCode(RTOCodes rtoCode);
        RTOCodes UpdateRTOCode(RTOCodes rtoCode);
        bool DeleteRTOCode(int id);
    }
}
