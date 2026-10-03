using Domain.Models;
using Domain.Optimization;
using MediatR;

namespace Application.Features.Optimization
{
    public sealed class OptimizeRecipesHandler
    : IRequestHandler<OptimizeRecipesCommand, OptimizeRecipesResult>
    {
        private readonly IRecipeOptimizer _optimizer;

        public OptimizeRecipesHandler(IRecipeOptimizer optimizer)
        {
            _optimizer = optimizer;
        }

        public Task<OptimizeRecipesResult> Handle(
            OptimizeRecipesCommand request,
            CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);

            cancellationToken.ThrowIfCancellationRequested();

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

            return Task.FromResult(
                new OptimizeRecipesResult(
                    allocations,
                    result.PeopleFed,
                    result.UnusedIngredients));
        }
    }
}
