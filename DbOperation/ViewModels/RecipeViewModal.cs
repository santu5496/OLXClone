using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbOperation.ViewModels
{
    public class RecipeViewModal
    {
        public int recipeId { get; set; }

        public int fkItemName { get; set; }

        public decimal? baseQuantity { get; set; }

        public string unit { get; set; }

        public DateTime? createdDate { get; set; }

        public DateTime? updatedDate { get; set; }

        public string sUser { get; set; }
        public string itemName { get; set; }
    }
}
