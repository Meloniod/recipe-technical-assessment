
namespace Application.Features.Optimization
{
    public sealed record OptimizeRecipesResult(
    IReadOnlyCollection<RecipeAllocationResult> Allocations,
    int PeopleFed,
    IReadOnlyDictionary<string, int> UnusedIngredients);

    public sealed record RecipeAllocationResult(
        string RecipeName,
        int Quantity,
        int PeopleFed);
}
