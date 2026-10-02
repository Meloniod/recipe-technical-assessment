using Domain.Models;

namespace Domain.Optimization
{
    public interface IRecipeOptimizer
    {
        OptimizationResult Optimize(
            IReadOnlyCollection<Recipe> recipes,
            IReadOnlyCollection<AvailableIngredient> availableIngredients);
    }
}
