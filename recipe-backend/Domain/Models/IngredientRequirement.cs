namespace Domain.Models
{
    public sealed record IngredientRequirement
    {
        public IngredientRequirement(string name, int quantity)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException(
                    "Ingredient name is required.",
                    nameof(name));
            }

            if (quantity <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(quantity),
                    "Ingredient quantity must be greater than zero.");
            }

            Name = name.Trim();
            Quantity = quantity;
        }

        public string Name { get; }

        public int Quantity { get; }
    }
}
