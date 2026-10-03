
using Domain.Models;
using MediatR;

namespace Application.Features.Optimization
{
    public sealed record OptimizeRecipesCommand(
    IReadOnlyCollection<Recipe> Recipes,
    IReadOnlyCollection<AvailableIngredient> AvailableIngredients)
    : IRequest<OptimizeRecipesResult>;
}
