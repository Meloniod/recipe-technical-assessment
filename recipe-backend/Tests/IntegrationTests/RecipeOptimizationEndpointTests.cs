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

        [Fact]
        public async Task Health_ReturnsHealthy()
        {
            var response = await _client.GetAsync("/health");

            Assert.Equal(
                HttpStatusCode.OK,
                response.StatusCode);

            var content =
                await response.Content.ReadAsStringAsync();

            Assert.Equal(
                "Healthy",
                content);
        }

        [Fact]
        public async Task Cors_PreflightAllowsConfiguredHttpsOrigin()
        {
            using var request = new HttpRequestMessage(
                HttpMethod.Options,
                "/api/v1/recipes/optimize");
            request.Headers.Add("Origin", "https://localhost:4200");
            request.Headers.Add("Access-Control-Request-Method", "POST");
            request.Headers.Add("Access-Control-Request-Headers", "content-type");

            var response = await _client.SendAsync(request);

            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
            Assert.Equal(
                "https://localhost:4200",
                response.Headers.GetValues("Access-Control-Allow-Origin").Single());
            Assert.Contains(
                "POST",
                response.Headers.GetValues("Access-Control-Allow-Methods").Single());
        }
    }
}
