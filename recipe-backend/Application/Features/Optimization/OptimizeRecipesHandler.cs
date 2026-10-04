using Domain.Models;
using Domain.Optimization;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Optimization
{
    public sealed class OptimizeRecipesHandler
    : IRequestHandler<OptimizeRecipesCommand, OptimizeRecipesResult>
    {
        private readonly IRecipeOptimizer _optimizer;
        private readonly ILogger<OptimizeRecipesHandler> _logger;

        public OptimizeRecipesHandler(
            IRecipeOptimizer optimizer,
            ILogger<OptimizeRecipesHandler> logger)
        {
            _optimizer = optimizer;
            _logger = logger;
        }

        public Task<OptimizeRecipesResult> Handle(
            OptimizeRecipesCommand request,
            CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);

            cancellationToken.ThrowIfCancellationRequested();

            _logger.LogInformation(
                "Starting recipe optimization. Recipes: {RecipeCount}, Available ingredients: {IngredientCount}",
                request.Recipes.Count,
                request.AvailableIngredients.Count);

            var recipes = request.Recipes
                .Select(recipe =>
                    new Recipe(
                        recipe.Name,
                        recipe.Servings,
                        recipe.Ingredients.Select(
                            ingredient =>
                                new IngredientRequirement(
                                    ingredient.Name,
                                    ingredient.Quantity))))
                .ToArray();

            var availableIngredients = request.AvailableIngredients
                .Select(ingredient =>
                    new AvailableIngredient(
                        ingredient.Name,
                        ingredient.Quantity))
                .ToArray();

            var result = _optimizer.Optimize(
                recipes,
                availableIngredients);

            var allocations = result.Allocations
                .Select(allocation =>
                    new RecipeAllocationResult(
                        allocation.RecipeName,
                        allocation.Quantity,
                        allocation.PeopleFed))
                .ToArray();

            _logger.LogInformation(
                "Recipe optimization completed. People fed: {PeopleFed}, Recipes selected: {RecipeCount}",
                result.PeopleFed,
                allocations.Length);

            return Task.FromResult(
                new OptimizeRecipesResult(
                    allocations,
                    result.PeopleFed,
                    result.UnusedIngredients));
        }
    }
}
