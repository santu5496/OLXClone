using DbOperation.Interface;
using DbOperation.Models;
using DbOperation.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DbOperation.Implementation
{
    public class RecipeService : IRecipeService
    {
        private readonly DbContextOptions<Assignment4Context> _dbOptions;

        public RecipeService(string dbConn)
        {
            _dbOptions = new DbContextOptionsBuilder<Assignment4Context>()
                .UseSqlServer(dbConn)
                .Options;
        }

        public bool AddRecipe(Recipe recipe, List<ItemsUsedForGoods> RecipeIngradientsUsed)
        {
            using var dbConn = new Assignment4Context(_dbOptions);
            try
            {
                recipe.createdDate = DateTime.Now;
                recipe.updatedDate = DateTime.Now;
                recipe.sUser = "User";
                dbConn.Add(recipe);
                dbConn.SaveChanges();

                foreach (var item in RecipeIngradientsUsed)
                {
                    var ItemsUsedForRecipe = new ItemsUsedForGoods
                    {
                        fkMasterItemId = item.fkMasterItemId,
                        fkBakeRecId = recipe.recipeId,
                        quantity = item.quantity,
                        unit = item.unit,
                        goodsType = "recipe"
                    };
                    dbConn.Add(ItemsUsedForRecipe);
                }
                dbConn.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return false;
            }
        }
        public bool EditRecipe(Recipe updatedRecipe, List<ItemsUsedForGoods> updatedIngredients)
        {
            using var dbConn = new Assignment4Context(_dbOptions);
            try
            {
                var existingRecipe = dbConn.Recipe.FirstOrDefault(r => r.recipeId == updatedRecipe.recipeId);

                if (existingRecipe == null)
                {
                    Console.WriteLine("Recipe not found.");
                    return false;
                }

                // Update existing recipe fields
               //existingRecipe.fk = updatedRecipe.recipeName;
                existingRecipe.baseQuantity = updatedRecipe.baseQuantity;
                existingRecipe.unit = updatedRecipe.unit;
                existingRecipe.updatedDate = DateTime.Now;
                existingRecipe.sUser = "User";

                dbConn.Update(existingRecipe);
                dbConn.SaveChanges();

                // Remove existing ingredients for this recipe
                var oldIngredients = dbConn.ItemsUsedForGoods.Where(i => i.fkBakeRecId == updatedRecipe.recipeId).ToList();
                dbConn.ItemsUsedForGoods.RemoveRange(oldIngredients);
                dbConn.SaveChanges();

                // Insert updated ingredients
                foreach (var item in updatedIngredients)
                {
                    var newIngredient = new ItemsUsedForGoods
                    {
                        fkMasterItemId = item.fkMasterItemId,
                        fkBakeRecId = updatedRecipe.recipeId,
                        quantity = item.quantity,
                        unit = "KG",
                        goodsType = "recipe"
                    };
                    dbConn.Add(newIngredient);
                }
                dbConn.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error updating recipe: " + ex.Message);
                return false;
            }
        }




        public bool DeleteRecipe(int recipeId)
        {
            using var dbConn = new Assignment4Context(_dbOptions);
            try
            {
                // Find the recipe by ID
                var recipe = dbConn.Recipe.FirstOrDefault(r => r.recipeId == recipeId);
                if (recipe == null)
                {
                    Console.WriteLine("Recipe not found.");
                    return false;
                }

                // Remove associated ingredients
                var ingredients = dbConn.ItemsUsedForGoods.Where(i => i.fkBakeRecId == recipeId &&i.goodsType== "recipe").ToList();
                dbConn.ItemsUsedForGoods.RemoveRange(ingredients);

                // Remove the recipe
                dbConn.Recipe.Remove(recipe);
                dbConn.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error deleting recipe: " + ex.Message);
                return false;
            }
        }


        public List<ItemsUsedForGoods> Getrecingradientsbyid(int id)
        {
            using var dbConn = new Assignment4Context(_dbOptions);
            return dbConn.ItemsUsedForGoods
                         .Where(x => x.fkBakeRecId == id && x.goodsType== "recipe")
                         .ToList();
        }

        public List<RecipeViewModal> GetRecipe()
        {
            using var dbConn = new Assignment4Context(_dbOptions);

            var recipesWithItems = (from r in dbConn.Recipe
                                    join i in dbConn.InventoryItems on r.fkItemName equals i.itemId 
                                    select new RecipeViewModal
                                    {
                                        recipeId = r.recipeId,
                                        baseQuantity = r.baseQuantity ?? 0,
                                        unit = r.unit,
                                        fkItemName = r.fkItemName,
                                        itemName = i.itemName
                                    }).ToList();

            return recipesWithItems;
        }
    }
}
