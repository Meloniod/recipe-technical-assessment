using Domain.Models;
using Domain.Optimization;

namespace UnitTests.RecipeOptimization
{
    public sealed class RecipeOptimizerTests
    {
        private readonly IRecipeOptimizer _optimizer =
            new RecipeOptimizer();

        [Fact]
        public void Optimize_MaximizesPeopleFed()
        {
            var recipes = new[]
            {
            new Recipe(
                "Sandwich",
                2,
                new[]
                {
                    new IngredientRequirement("Bread", 2),
                    new IngredientRequirement("Lettuce", 1)
                }),

                new Recipe(
                    "Salad",
                    1,
                    new[]
                    {
                        new IngredientRequirement("Lettuce", 2)
                    })
            };

                var ingredients = new[]
                {
                new AvailableIngredient("Bread", 4),
                new AvailableIngredient("Lettuce", 3)
            };

            var result =
                _optimizer.Optimize(
                    recipes,
                    ingredients);

            Assert.Equal(4, result.PeopleFed);

            Assert.Single(result.Allocations);

            Assert.Equal(
                "Sandwich",
                result.Allocations[0].RecipeName);

            Assert.Equal(
                2,
                result.Allocations[0].Quantity);
        }

        [Fact]
        public void Optimize_DoesNotUseMoreIngredientsThanAvailable()
        {
            var recipes = new[]
            {
                new Recipe(
                    "Meal",
                    3,
                    new[]
                    {
                        new IngredientRequirement("Rice", 2),
                        new IngredientRequirement("Chicken", 1)
                    })
            };

            var ingredients = new[]
            {
                new AvailableIngredient("Rice", 5),
                new AvailableIngredient("Chicken", 2)
            };

            var result =
                _optimizer.Optimize(
                    recipes,
                    ingredients);

            Assert.Equal(6, result.PeopleFed);

            Assert.Equal(
                1,
                result.UnusedIngredients["Rice"]);

            Assert.Equal(
                0,
                result.UnusedIngredients["Chicken"]);
        }

        [Fact]
        public void Optimize_CannotProduceRecipeWhenIngredientIsUnavailable()
        {
            var recipes = new[]
            {
            new Recipe(
                "Burger",
                1,
                new[]
                {
                    new IngredientRequirement("Bread", 1),
                    new IngredientRequirement("Beef", 1)
                })
        };

            var ingredients = new[]
            {
            new AvailableIngredient("Bread", 10)
        };

            var result =
                _optimizer.Optimize(
                    recipes,
                    ingredients);

            Assert.Equal(0, result.PeopleFed);
            Assert.Empty(result.Allocations);
        }

        [Fact]
        public void Optimize_IngredientNamesAreCaseInsensitive()
        {
            var recipes = new[]
            {
            new Recipe(
                "Salad",
                1,
                new[]
                {
                    new IngredientRequirement("lettuce", 2)
                })
        };

            var ingredients = new[]
            {
            new AvailableIngredient("LETTUCE", 4)
        };

            var result =
                _optimizer.Optimize(
                    recipes,
                    ingredients);

            Assert.Equal(2, result.PeopleFed);
        }

        [Fact]
        public void Optimize_ZeroAvailableQuantityProducesNothing()
        {
            var recipes = new[]
            {
            new Recipe(
                "Salad",
                1,
                new[]
                {
                    new IngredientRequirement("Lettuce", 1)
                })
        };

            var ingredients = new[]
            {
            new AvailableIngredient("Lettuce", 0)
        };

            var result =
                _optimizer.Optimize(
                    recipes,
                    ingredients);

            Assert.Equal(0, result.PeopleFed);
            Assert.Empty(result.Allocations);
        }

        [Fact]
        public void Optimize_RejectsDuplicateAvailableIngredients()
        {
            var recipes = new[]
            {
            new Recipe(
                "Salad",
                1,
                new[]
                {
                    new IngredientRequirement("Lettuce", 1)
                })
        };

            var ingredients = new[]
            {
            new AvailableIngredient("Lettuce", 2),
            new AvailableIngredient("lettuce", 3)
        };

            Assert.Throws<ArgumentException>(
                () => _optimizer.Optimize(
                    recipes,
                    ingredients));
        }
    }
}
