namespace Domain.Models
{
    public sealed class OptimizationResult
    {
        public OptimizationResult(
            IEnumerable<RecipeAllocation> allocations,
            int peopleFed,
            IReadOnlyDictionary<string, int> unusedIngredients)
        {
            ArgumentNullException.ThrowIfNull(allocations);
            ArgumentNullException.ThrowIfNull(unusedIngredients);

            Allocations = Array.AsReadOnly(
                allocations
                    .Where(allocation => allocation.Quantity > 0)
                    .ToArray());

            PeopleFed = peopleFed;

            UnusedIngredients = new Dictionary<string, int>(
                unusedIngredients,
                StringComparer.OrdinalIgnoreCase);
        }

        public IReadOnlyList<RecipeAllocation> Allocations { get; }

        public int PeopleFed { get; }

        public IReadOnlyDictionary<string, int> UnusedIngredients { get; }
    }
}
