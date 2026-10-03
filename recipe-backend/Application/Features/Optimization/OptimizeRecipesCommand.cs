using MediatR;

namespace Application.Features.Optimization
{
    public sealed record OptimizeRecipesCommand(
    IReadOnlyCollection<RecipeInput> Recipes,
    IReadOnlyCollection<AvailableIngredientInput> AvailableIngredients)
    : IRequest<OptimizeRecipesResult>;

    public sealed record RecipeInput(
        string Name,
        int Servings,
        IReadOnlyCollection<IngredientRequirementInput> Ingredients);

    public sealed record IngredientRequirementInput(
        string Name,
        int Quantity);

    public sealed record AvailableIngredientInput(
        string Name,
        int Quantity);
}
