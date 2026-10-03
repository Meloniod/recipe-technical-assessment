namespace Api.Contracts.Optimization
{
    public sealed record OptimizeRecipesResponse(
    IReadOnlyCollection<RecipeAllocationResponse> Allocations,
    int PeopleFed,
    IReadOnlyDictionary<string, int> UnusedIngredients);

    public sealed record RecipeAllocationResponse(
        string RecipeName,
        int Quantity,
        int PeopleFed);
}
