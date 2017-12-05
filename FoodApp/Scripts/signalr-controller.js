var recipeHub = $.connection.recipeHub;

$.connection.hub.start().done(function () {
});

// Event Listener
// Receives Events From Server When New Recipe is Created
recipeHub.client.newRecipe = function (newRecipe) {
    renderRecipeModal(newRecipe);
}

//-------------------------------------------------------------------
// Helper Functions
function renderRecipeModal(recipe) {
    // bind recipe data
    $("#new-recipe-title").text(recipe.title);
    $("#new-recipe-picture").attr("src", recipe.picture);
    $("#new-recipe-description").text(recipe.description);
    $(".new-recipe-link").attr("href", "/Recipes/Details/"+recipe.id);

    // show modal
    $('#myModal').modal('show');
}
