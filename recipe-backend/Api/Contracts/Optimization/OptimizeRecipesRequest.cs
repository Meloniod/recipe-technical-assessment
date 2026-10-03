namespace Api.Contracts.Optimization
{
    public sealed record OptimizeRecipesRequest(
    IReadOnlyCollection<AvailableIngredientRequest> AvailableIngredients,
    IReadOnlyCollection<RecipeRequest> Recipes);

    public sealed record AvailableIngredientRequest(
        string Name,
        int Quantity);

    public sealed record RecipeRequest(
        string Name,
        int Servings,
        IReadOnlyCollection<IngredientRequirementRequest> Ingredients);

    public sealed record IngredientRequirementRequest(
        string Name,
        int Quantity);
}
