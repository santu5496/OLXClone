using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbOperation.ViewModels
{
    public class CarModelNamveVewModel
    {
        public int brandId { get; set; }
        public string brandName { get; set; }
        public string brandCountryOfOrigin { get; set; }
        public string brandWebsite { get; set; }
        public bool isLuxuryBrand { get; set; }
        public bool isActiveBrand { get; set; }
        public string brandNotes { get; set; }
        public int modelId { get; set; }

       

        public string modelName { get; set; }

        public int? modelLaunchYear { get; set; }

        public int? modelDiscontinuedYear { get; set; }

        public string modelGeneration { get; set; }

        public string modelBodyType { get; set; }

        public bool? isActiveModel { get; set; }

        public DateTime? modelCreatedDate { get; set; }

        public string modelDescription { get; set; }
    }
}
