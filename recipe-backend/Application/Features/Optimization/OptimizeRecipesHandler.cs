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

            var result = _optimizer.Optimize(
                request.Recipes,
                request.AvailableIngredients);

            var allocations = result.Allocations
                .Select(allocation =>
                    new RecipeAllocationResult(
                        allocation.RecipeName,
                        allocation.Quantity,
                        allocation.PeopleFed))
                .ToArray();

            var response = new OptimizeRecipesResult(
                allocations,
                result.PeopleFed,
                result.UnusedIngredients);

            return Task.FromResult(response);
        }
    }
}
