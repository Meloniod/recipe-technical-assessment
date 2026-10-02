namespace Domain.Models
{
    public sealed record AvailableIngredient
    {
        public AvailableIngredient(string name, int quantity)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException(
                    "Ingredient name is required.",
                    nameof(name));
            }

            if (quantity < 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(quantity),
                    "Available ingredient quantity cannot be negative.");
            }

            Name = name.Trim();
            Quantity = quantity;
        }

        public string Name { get; }

        public int Quantity { get; }
    }
}
