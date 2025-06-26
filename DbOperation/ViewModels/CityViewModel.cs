using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbOperation.ViewModels
{
    public class CityViewModel
    {
        public int cityId { get; set; }
        public int stateId { get; set; }
        public string cityName { get; set; }
        public string cityType { get; set; }
        public int? cityPopulation { get; set; }
        public bool hasGoodCarMarket { get; set; }
        public string typicalCarDemand { get; set; }
        public bool isActiveCity { get; set; }
        public string stateName { get; set; }
    }
}

