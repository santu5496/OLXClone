using DbOperation.Interface;
using DbOperation.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using ZXing;

namespace Assignment4.Controllers
{
    public class Configuration1Controller : Controller
    {
        private readonly IConfigurationService1 _dbConn;

        public Configuration1Controller(IConfigurationService1 conn)
        {
            _dbConn = conn;
        }

        public IActionResult Configuration1()
        {
            return View();
        }

        // ------------------- CAR COLORS -------------------

        [HttpGet]
        public IActionResult GetCarColors()
        {
            try
            {
                var data = _dbConn.GetAllColours();
                return Json(data);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult AddOrEditCarColor(CarColors data)
        {
            try
            {
                if(data.colorId==0)
                {
                  var result = _dbConn.AddCarColor(data);
                    return Json(result != null ? new { success = true, data = result } : new { success = false });
                }
                else
                {
                               var result=        _dbConn.UpdateCarColor(data);
                    return Json(result ? new { success = true } : new { success = false });
                }



               
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult DeleteCarColor(int id)
        {
            try
            {
                var result = _dbConn.DeleteCarColor(id);
                return Json(new { success = result });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

        // ------------------- CAR CONDITIONS -------------------

        [HttpGet]
        public IActionResult GetCarConditions()
        {
            try
            {
                var data = _dbConn.GetAllCarConditions();
                return Json(data);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult AddOrEditCarCondition(CarConditionLevels model)
        {
            try
            {
                var result = model.conditionId == 0
                    ? _dbConn.AddCarCondition(model)
                    : _dbConn.UpdateCarCondition(model);

                return Json(result != null ? new { success = true, data = result } : new { success = false });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult DeleteCarCondition(int id)
        {
            try
            {
                var result = _dbConn.DeleteCarCondition(id);
                return Json(new { success = result });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

        // ------------------- STATES -------------------

        [HttpGet]
        public IActionResult GetAllStates()
        {
            try
            {
                var data = _dbConn.GetAllStates();
                return Json(data);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult AddOrEditState(GeographicStates model)
        {
            try
            {
                if(model.stateId==0)
                {
                    bool a=_dbConn.AddState(model);
                    return Json(new { success = a });
                }
                else
                {
                    var result = _dbConn.UpdateState(model);
                    return Json(new { success = result });
                    
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult DeleteState(int id)
        {
            try
            {
                var result = _dbConn.DeleteState(id);
                return Json(new { success = result });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

        // ------------------- CITIES -------------------

        [HttpGet]
        public IActionResult GetAllCities()
        {
            try
            {
                var data = _dbConn.GetAllCities();
                return Json(data);
           
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult AddOrEditCity(GeographicCities model)
        {
            try
            {
                var result = model.cityId == 0
                    ? _dbConn.AddCity(model)
                    : _dbConn.UpdateCity(model);

                return Json(result != null ? new { success = true, data = result } : new { success = false });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult DeleteCity(int id)
        {
            try
            {
                return Json(new { success = _dbConn.DeleteCity(id) });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

        // ------------------- ENGINE SPECS -------------------

        [HttpGet]
        public IActionResult GetAllEngineSpecs()
        {
            try
            {
                return Json(_dbConn.GetAllEngineSpecs());
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult AddOrEditEngineSpec(EngineSpecifications data)
        {
            try
            {
               

                if (data.engineSpecId==null)
                {
                   var result= _dbConn.AddEngineSpec(data);
                    return Json(result != null ? new { success = true, data = result } : new { success = false });
                }
                else if(data.engineSpecId !=null)
                {
                    var result  =  _dbConn.UpdateEngineSpec(data);
                    return Json(result != null ? new { success = true, data = result } : new { success = false });
                }

                return Json(true);

            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
                return Json(false);
            }
        }

        [HttpPost]
        public IActionResult DeleteEngineSpec(int id)
        {
            try
            {
                return Json(new { success = _dbConn.DeleteEngineSpec(id) });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

        // ------------------- FEATURES -------------------

        [HttpGet]
        public IActionResult GetAllCarFeatures()
        {
            try
            {
                return Json(_dbConn.GetAllCarFeatures());
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult AddOrEditCarFeature(CarFeaturesList model)
        {
            try
            {
                var result = model.featureId == 0
                    ? _dbConn.AddCarFeature(model)
                    : _dbConn.UpdateCarFeature(model);

                return Json(result != null ? new { success = true, data = result } : new { success = false });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult DeleteCarFeature(int id)
        {
            try
            {
                return Json(new { success = _dbConn.DeleteCarFeature(id) });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

        // ------------------- INSURANCE PROVIDERS -------------------

        [HttpGet]
        public IActionResult GetAllInsuranceProviders()
        {
            try
            {
                return Json(_dbConn.GetAllInsuranceProviders());
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult AddOrEditInsuranceProvider(InsuranceProviders model)
        {
            try
            {
                var result = model.insuranceProviderId == 0
                    ? _dbConn.AddInsuranceProvider(model)
                    : _dbConn.UpdateInsuranceProvider(model);

                return Json(result != null ? new { success = true, data = result } : new { success = false });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult DeleteInsuranceProvider(int id)
        {
            try
            {
                return Json(new { success = _dbConn.DeleteInsuranceProvider(id) });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

        // ------------------- RTO CODES -------------------

       [HttpGet]
        //public IActionResult GetAllRTOCodes()
        //{
        //    try
        //    {
        //        var rtoCodes = _dbConn.GetAllRTOCodes();
        //        if (rtoCodes == null)
        //        {
        //            return Json(new { success = false, error = "No RTO codes found." });
        //        }
        //        return Json(rtoCodes);
        //    }
        //    catch (Exception ex)
        //    {
        //        // Log the exception
        //        Console.WriteLine($"Error in GetAllRTOCodes: {ex.Message}");
        //        return Json(new { success = false, error = ex.Message });
        //    }
        //}
        [HttpGet]

        public JsonResult GetAllRTOCodes()
        {
            try
            {
                var rtoCodes = _dbConn.GetAllRTOCodes();
                return Json(new
                {
                    success = true,
                    data = rtoCodes
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetAllRTOCodes: {ex.Message}");
                return Json(new
                {
                    success = false,
                    error = ex.Message
                });
            }
        }

        [HttpPost]
        public IActionResult AddOrEditRTOCode(RTOCodes model)
        {
            try
            {
                var result = model.rtoId == 0
                    ? _dbConn.AddRTOCode(model)
                    : _dbConn.UpdateRTOCode(model);

                return Json(result != null ? new { success = true, data = result } : new { success = false });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }
        // Controller Method
        [HttpGet]

        public IActionResult GetCitiesByState(int stateId)
        {
            try
            {
                if (stateId <= 0)
                {
                    return Json(new { success = false, error = "Invalid state ID." });
                }

                var cities = _dbConn.GetCitiesByStateId(stateId);

                return Json(new { success = true, data = cities });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetCitiesByState: {ex.Message}");
                return Json(new { success = false, error = ex.Message });
            }
        }


        [HttpPost]
        public IActionResult DeleteRTOCode(int id)
        {
            try
            {
                return Json(new { success = _dbConn.DeleteRTOCode(id) });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }
    }
}
