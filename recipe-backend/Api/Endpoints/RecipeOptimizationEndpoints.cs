using Api.Contracts.Optimization;
using Api.Mapping;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Api.Endpoints
{
    public static class RecipeOptimizationEndpoints
    {
        public static IEndpointRouteBuilder MapRecipeOptimizationEndpoints(
            this IEndpointRouteBuilder endpoints)
        {
            endpoints.MapPost(
                    "/api/v1/recipes/optimize",
                    OptimizeRecipes)
                .WithName("OptimizeRecipes")
                .WithTags("Recipe Optimization")
                .Produces<OptimizeRecipesResponse>(
                    StatusCodes.Status200OK)
                .ProducesProblem(
                    StatusCodes.Status400BadRequest)
                .ProducesProblem(
                    StatusCodes.Status500InternalServerError);

            return endpoints;
        }

        private static async Task<
            Results<
                Ok<OptimizeRecipesResponse>,
                ProblemHttpResult>>
            OptimizeRecipes(
                OptimizeRecipesRequest request,
                ISender sender,
                CancellationToken cancellationToken)
        {
            var command = request.ToCommand();

            var result = await sender.Send(
                command,
                cancellationToken);

            return TypedResults.Ok(
                result.ToResponse());
        }
    }
}
