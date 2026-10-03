using Api.Contracts.Optimization;
using IntegrationTests.Factory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTests
{
    public sealed class RecipeOptimizationEndpointTests
    : IClassFixture<RecipeOptimizationApiFactory>
    {
        private readonly HttpClient _client;

        public RecipeOptimizationEndpointTests(
            RecipeOptimizationApiFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task OptimizeRecipes_ReturnsOptimalCombination()
        {
            var request = new OptimizeRecipesRequest(
                new[]
                {
                new AvailableIngredientRequest(
                    "Bread",
                    4),

                new AvailableIngredientRequest(
                    "Lettuce",
                    3)
                },
                new[]
                {
                new RecipeRequest(
                    "Sandwich",
                    2,
                    new[]
                    {
                        new IngredientRequirementRequest(
                            "Bread",
                            2),

                        new IngredientRequirementRequest(
                            "Lettuce",
                            1)
                    }),

                new RecipeRequest(
                    "Salad",
                    1,
                    new[]
                    {
                        new IngredientRequirementRequest(
                            "Lettuce",
                            2)
                    })
                });

            var response = await _client.PostAsJsonAsync(
                "/api/v1/recipes/optimize",
                request);

            Assert.Equal(
                HttpStatusCode.OK,
                response.StatusCode);

            var result =
                await response.Content
                    .ReadFromJsonAsync<OptimizeRecipesResponse>();

            Assert.NotNull(result);

            Assert.Equal(
                4,
                result.PeopleFed);

            Assert.Single(result.Allocations);

            Assert.Equal(
                "Sandwich",
                result.Allocations.Single().RecipeName);

            Assert.Equal(
                2,
                result.Allocations.Single().Quantity);
        }
        [Fact]
        public async Task OptimizeRecipes_WithNoRecipes_ReturnsBadRequest()
        {
            var request = new OptimizeRecipesRequest(
                new[]
                {
            new AvailableIngredientRequest(
                "Bread",
                4)
                },
                Array.Empty<RecipeRequest>());

            var response = await _client.PostAsJsonAsync(
                "/api/v1/recipes/optimize",
                request);

            Assert.Equal(
                HttpStatusCode.BadRequest,
                response.StatusCode);
        }
        [Fact]
        public async Task OptimizeRecipes_WithInvalidIngredientQuantity_ReturnsBadRequest()
        {
            var request = new OptimizeRecipesRequest(
                new[]
                {
            new AvailableIngredientRequest(
                "Bread",
                -1)
                },
                new[]
                {
            new RecipeRequest(
                "Sandwich",
                2,
                new[]
                {
                    new IngredientRequirementRequest(
                        "Bread",
                        1)
                })
                });

            var response = await _client.PostAsJsonAsync(
                "/api/v1/recipes/optimize",
                request);

            Assert.Equal(
                HttpStatusCode.BadRequest,
                response.StatusCode);
        }
    }
}
