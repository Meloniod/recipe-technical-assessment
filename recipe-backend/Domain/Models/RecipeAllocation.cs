namespace Domain.Models
{
    public sealed record RecipeAllocation(
    string RecipeName,
    int Quantity,
    int PeopleFed);
}
