using Application.Features.Optimization;
using Domain.Models;
using Domain.Optimization;

namespace UnitTests.Optimization
{
    public sealed class OptimizeRecipesHandlerTests
    {
        [Fact]
        public async Task Handle_DelegatesOptimizationToDomainService()
        {
            var optimizer = new StubRecipeOptimizer();

            var handler =
                new OptimizeRecipesHandler(optimizer);

            var command = new OptimizeRecipesCommand(
                new[]
                {
                new Recipe(
                    "Sandwich",
                    2,
                    new[]
                    {
                        new IngredientRequirement("Bread", 2)
                    })
                },
                new[]
                {
                new AvailableIngredient("Bread", 4)
                });

            var result = await handler.Handle(
                command,
                CancellationToken.None);

            Assert.Equal(2, result.PeopleFed);
            Assert.Single(result.Allocations);

            Assert.Equal(
                "Sandwich",
                result.Allocations.Single().RecipeName);

            Assert.Equal(
                1,
                result.Allocations.Single().Quantity);
        }

        [Fact]
        public async Task Handle_PropagatesCancellation()
        {
            var optimizer = new StubRecipeOptimizer();

            var handler =
                new OptimizeRecipesHandler(optimizer);

            using var cancellationTokenSource =
                new CancellationTokenSource();

            cancellationTokenSource.Cancel();

            var command = new OptimizeRecipesCommand(
                Array.Empty<Recipe>(),
                Array.Empty<AvailableIngredient>());

            await Assert.ThrowsAsync<OperationCanceledException>(
                () => handler.Handle(
                    command,
                    cancellationTokenSource.Token));
        }

        private sealed class StubRecipeOptimizer : IRecipeOptimizer
        {
            public OptimizationResult Optimize(
                IReadOnlyCollection<Recipe> recipes,
                IReadOnlyCollection<AvailableIngredient> availableIngredients)
            {
                return new OptimizationResult(
                    new[]
                    {
                    new RecipeAllocation(
                        "Sandwich",
                        1,
                        2)
                    },
                    2,
                    new Dictionary<string, int>
                    {
                        ["Bread"] = 2
                    });
            }
        }
    }
}
