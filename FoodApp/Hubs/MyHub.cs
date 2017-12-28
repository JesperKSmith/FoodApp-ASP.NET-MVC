using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using FoodApp.Models;

namespace FoodApp.SignalHubs
{
    public class RecipeHub : Hub
    {

        // INFORM ALL CLIENTS ABOUT NEW RECIPE
        public void InformClientAboutNewRecipe(Recipe recipe)
        {
            var context = GlobalHost.ConnectionManager.GetHubContext<RecipeHub>();
            context.Clients.All.newRecipe(getRecieModalView(recipe));

        }

        private object getRecieModalView(Recipe recipe)
        {
            return new
            {
                id = recipe.Id,
                title = recipe.Title,
                description = recipe.Description,
                picture = recipe.Picture.Substring(1)
            };
        }
    }
}