using Application.Features.Optimization;
using Domain.Models;
using Domain.Optimization;
using Microsoft.Extensions.Logging;

namespace UnitTests.Optimization
{
    public sealed class OptimizeRecipesHandlerTests
    {
        [Fact]
        public async Task Handle_DelegatesOptimizationToDomainService()
        {
            var optimizer = new StubRecipeOptimizer();

            var handler = CreateHandler(optimizer);

            var command = new OptimizeRecipesCommand(
                new[]
                {
                    new RecipeInput(
                        "Sandwich",
                        2,
                        new[]
                        {
                            new IngredientRequirementInput(
                                "Bread",
                                2)
                        })
                },
                new[]
                {
                    new AvailableIngredientInput(
                        "Bread",
                        4)
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

            var handler = CreateHandler(optimizer);

            using var cancellationTokenSource =
                new CancellationTokenSource();

            cancellationTokenSource.Cancel();

            var command = new OptimizeRecipesCommand(
                Array.Empty<RecipeInput>(),
                Array.Empty<AvailableIngredientInput>());

            await Assert.ThrowsAsync<OperationCanceledException>(
                () => handler.Handle(
                    command,
                    cancellationTokenSource.Token));
        }

        private static OptimizeRecipesHandler CreateHandler(IRecipeOptimizer optimizer)
        {
            var loggerFactory =
                LoggerFactory.Create(_ => { });

            var logger =
                loggerFactory.CreateLogger<OptimizeRecipesHandler>();

            return new OptimizeRecipesHandler(
                optimizer,
                logger);
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
