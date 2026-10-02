using Domain.Models;

namespace Domain.Optimization
{
    public sealed class RecipeOptimizer : IRecipeOptimizer
    {
        public OptimizationResult Optimize(
            IReadOnlyCollection<Recipe> recipes,
            IReadOnlyCollection<AvailableIngredient> availableIngredients)
        {
            ArgumentNullException.ThrowIfNull(recipes);
            ArgumentNullException.ThrowIfNull(availableIngredients);

            var stock = ToDictionary(availableIngredients);

            if (recipes.Count == 0)
            {
                return new OptimizationResult(
                    [],
                    0,
                    stock);
            }

            /*
             * A recipe that requires an ingredient that isn't available
             * can never be produced.
             */
            var usableRecipes = recipes
                .Where(recipe =>
                    recipe.Ingredients.All(
                        ingredient => stock.ContainsKey(ingredient.Name)))
                .ToArray();

            if (usableRecipes.Length == 0)
            {
                return new OptimizationResult(
                    [],
                    0,
                    stock);
            }

            var remaining = new Dictionary<string, int>(
                stock,
                StringComparer.OrdinalIgnoreCase);

            var currentQuantities = new int[usableRecipes.Length];
            var bestQuantities = new int[usableRecipes.Length];

            var bestPeopleFed = 0;

            /*
             * This is an optimistic upper bound used for pruning.
             *
             * It intentionally ignores competition for ingredients, so it can
             * overestimate what is achievable but can never underestimate it.
             */
            var suffixUpperBounds =
                BuildSuffixUpperBounds(usableRecipes, stock);

            Search(
                recipeIndex: 0,
                peopleFed: 0);

            var allocations = usableRecipes
                .Select(
                    (recipe, index) =>
                        new RecipeAllocation(
                            recipe.Name,
                            bestQuantities[index],
                            checked(
                                bestQuantities[index] *
                                recipe.Servings)))
                .Where(allocation => allocation.Quantity > 0)
                .ToArray();

            var unusedIngredients = new Dictionary<string, int>(
                stock,
                StringComparer.OrdinalIgnoreCase);

            for (var i = 0; i < usableRecipes.Length; i++)
            {
                var recipeQuantity = bestQuantities[i];

                foreach (var ingredient in usableRecipes[i].Ingredients)
                {
                    unusedIngredients[ingredient.Name] -=
                        checked(recipeQuantity * ingredient.Quantity);
                }
            }

            return new OptimizationResult(
                allocations,
                bestPeopleFed,
                unusedIngredients);

            void Search(
                int recipeIndex,
                int peopleFed)
            {
                if (recipeIndex == usableRecipes.Length)
                {
                    if (peopleFed > bestPeopleFed)
                    {
                        bestPeopleFed = peopleFed;

                        Array.Copy(
                            currentQuantities,
                            bestQuantities,
                            currentQuantities.Length);
                    }

                    return;
                }

                /*
                 * If even the optimistic maximum cannot beat the current
                 * solution, there is no reason to explore this branch.
                 */
                if (peopleFed + suffixUpperBounds[recipeIndex]
                    <= bestPeopleFed)
                {
                    return;
                }

                var recipe = usableRecipes[recipeIndex];

                var maximumQuantity =
                    GetMaximumQuantity(recipe, remaining);

                /*
                 * Larger quantities are tried first so that good solutions
                 * are found early, improving pruning.
                 */
                for (
                    var quantity = maximumQuantity;
                    quantity >= 0;
                    quantity--)
                {
                    Consume(
                        recipe,
                        quantity,
                        remaining);

                    currentQuantities[recipeIndex] = quantity;

                    Search(
                        recipeIndex + 1,
                        peopleFed +
                        checked(quantity * recipe.Servings));

                    Restore(
                        recipe,
                        quantity,
                        remaining);
                }

                currentQuantities[recipeIndex] = 0;
            }
        }

        private static int[] BuildSuffixUpperBounds(
            IReadOnlyList<Recipe> recipes,
            IReadOnlyDictionary<string, int> stock)
        {
            var result = new int[recipes.Count + 1];

            for (var i = recipes.Count - 1; i >= 0; i--)
            {
                var maximumQuantity =
                    GetMaximumQuantity(
                        recipes[i],
                        stock);

                result[i] =
                    checked(
                        result[i + 1] +
                        maximumQuantity *
                        recipes[i].Servings);
            }

            return result;
        }

        private static int GetMaximumQuantity(
            Recipe recipe,
            IReadOnlyDictionary<string, int> available)
        {
            var maximumQuantity = int.MaxValue;

            foreach (var ingredient in recipe.Ingredients)
            {
                if (!available.TryGetValue(
                        ingredient.Name,
                        out var availableQuantity))
                {
                    return 0;
                }

                maximumQuantity =
                    Math.Min(
                        maximumQuantity,
                        availableQuantity / ingredient.Quantity);
            }

            return maximumQuantity == int.MaxValue
                ? 0
                : maximumQuantity;
        }

        private static void Consume(
            Recipe recipe,
            int quantity,
            IDictionary<string, int> remaining)
        {
            foreach (var ingredient in recipe.Ingredients)
            {
                remaining[ingredient.Name] -=
                    checked(quantity * ingredient.Quantity);
            }
        }

        private static void Restore(
            Recipe recipe,
            int quantity,
            IDictionary<string, int> remaining)
        {
            foreach (var ingredient in recipe.Ingredients)
            {
                remaining[ingredient.Name] +=
                    checked(quantity * ingredient.Quantity);
            }
        }

        private static Dictionary<string, int> ToDictionary(
            IEnumerable<AvailableIngredient> ingredients)
        {
            var result =
                new Dictionary<string, int>(
                    StringComparer.OrdinalIgnoreCase);

            foreach (var ingredient in ingredients)
            {
                if (!result.TryAdd(
                        ingredient.Name,
                        ingredient.Quantity))
                {
                    throw new ArgumentException(
                        $"Duplicate available ingredient: '{ingredient.Name}'.");
                }
            }

            return result;
        }
    }
}
