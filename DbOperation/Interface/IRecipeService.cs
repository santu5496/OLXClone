using DbOperation.Models;
using DbOperation.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DbOperation.Implementation.RecipeService;

namespace DbOperation.Interface
{
    public interface IRecipeService
    {
        bool AddRecipe(Recipe recipe, List<ItemsUsedForGoods> RecipeIngradientsUsed);
        List<ItemsUsedForGoods> Getrecingradientsbyid(int id);
        List<RecipeViewModal> GetRecipe();
        bool EditRecipe(Recipe updatedRecipe, List<ItemsUsedForGoods> updatedIngredients);
        bool DeleteRecipe(int recipeId);
    }
}
