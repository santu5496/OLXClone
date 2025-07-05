using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbOperation.ViewModels
{
    // ViewModel for RTO Codes
    public class RTOCodeViewModel
    {
        public int rtoId { get; set; }
        public string rtoCode { get; set; }
        public string rtoName { get; set; }
        public int stateId { get; set; }
        public string stateName { get; set; }
        public int? cityId { get; set; }
        public string cityName { get; set; }
        public string rtoAddress { get; set; }
        public string rtoContactNumber { get; set; }
        public bool isActiveRTO { get; set; }
    }
}
