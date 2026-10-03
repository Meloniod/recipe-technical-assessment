using Api.Contracts.Optimization;
using Application.Features.Optimization;

namespace Api.Mapping
{
    public static class OptimizationMapping
    {
        public static OptimizeRecipesCommand ToCommand(
            this OptimizeRecipesRequest request)
        {
            ArgumentNullException.ThrowIfNull(request);

            var recipes = request.Recipes
                .Select(recipe =>
                    new RecipeInput(
                        recipe.Name,
                        recipe.Servings,
                        recipe.Ingredients
                            .Select(ingredient =>
                                new IngredientRequirementInput(
                                    ingredient.Name,
                                    ingredient.Quantity))
                            .ToArray()))
                .ToArray();

            var availableIngredients = request.AvailableIngredients
                .Select(ingredient =>
                    new AvailableIngredientInput(
                        ingredient.Name,
                        ingredient.Quantity))
                .ToArray();

            return new OptimizeRecipesCommand(
                recipes,
                availableIngredients);
        }

        public static OptimizeRecipesResponse ToResponse(
            this OptimizeRecipesResult result)
        {
            ArgumentNullException.ThrowIfNull(result);

            var allocations = result.Allocations
                .Select(allocation =>
                    new RecipeAllocationResponse(
                        allocation.RecipeName,
                        allocation.Quantity,
                        allocation.PeopleFed))
                .ToArray();

            return new OptimizeRecipesResponse(
                allocations,
                result.PeopleFed,
                result.UnusedIngredients);
        }
    }
}
