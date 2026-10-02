namespace Domain.Models
{
    public sealed class Recipe
    {
        public Recipe(
            string name,
            int servings,
            IEnumerable<IngredientRequirement> ingredients)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException(
                    "Recipe name is required.",
                    nameof(name));
            }

            if (servings <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(servings),
                    "Recipe servings must be greater than zero.");
            }

            ArgumentNullException.ThrowIfNull(ingredients);

            var requirements = ingredients.ToArray();

            if (requirements.Length == 0)
            {
                throw new ArgumentException(
                    "A recipe must contain at least one ingredient.",
                    nameof(ingredients));
            }

            var duplicateIngredient = requirements
                .GroupBy(
                    ingredient => ingredient.Name,
                    StringComparer.OrdinalIgnoreCase)
                .FirstOrDefault(group => group.Count() > 1);

            if (duplicateIngredient is not null)
            {
                throw new ArgumentException(
                    $"Recipe cannot contain ingredient '{duplicateIngredient.Key}' more than once.",
                    nameof(ingredients));
            }

            Name = name.Trim();
            Servings = servings;
            Ingredients = Array.AsReadOnly(requirements);
        }

        public string Name { get; }

        public int Servings { get; }

        public IReadOnlyList<IngredientRequirement> Ingredients { get; }
    }
}
