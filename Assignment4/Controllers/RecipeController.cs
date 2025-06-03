using DbOperation.Interface;
using Microsoft.AspNetCore.Mvc;
using DbOperation.Models;
using DbOperation.ViewModels;
using System.Security.Cryptography;
namespace Assignment4.Controllers
{
    public class RecipeController : Controller
    {
        private readonly IRecipeService _dbConn;
        public RecipeController(IRecipeService conn)
        {
            _dbConn = conn;
        }
        public IActionResult RecipeData()
        {
            return View();
        }
        
        
        public bool AddRecipe(Recipe item,List<ItemsUsedForGoods> RecipeIngradientsUsed)
        {
            try
            {
                if (item.recipeId == 0)
                {


                    bool result = _dbConn.AddRecipe(item, RecipeIngradientsUsed);
                    return result;
                }
                else
                {

                    bool result = _dbConn.EditRecipe(item, RecipeIngradientsUsed);
                    return result;
                }
            }
            catch
            {
                return false;
            }
        }
        public IActionResult GetIngredients(int id)
        {
            try
            {
                var List = _dbConn.Getrecingradientsbyid(id);
                return Json(List);
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }


        }
        public IActionResult GetRecipe()
        {
            try
            {
                var recipes = _dbConn.GetRecipe(); // Ensure this returns List<RecipeViewModal>
                return Json(recipes); // Return a JSON array
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }
        public IActionResult DeleteRecipe(int id)
        {
            try
            {
                
                var recipes = _dbConn.DeleteRecipe(id); // Ensure this returns List<RecipeViewModal>
                return Json(recipes); // Return a JSON array
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }


    }
}
