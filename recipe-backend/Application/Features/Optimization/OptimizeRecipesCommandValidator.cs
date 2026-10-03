using FluentValidation;

namespace Application.Features.Optimization
{
    public sealed class OptimizeRecipesCommandValidator
    : AbstractValidator<OptimizeRecipesCommand>
    {
        public OptimizeRecipesCommandValidator()
        {
            RuleFor(command => command.AvailableIngredients)
                .NotNull()
                .NotEmpty()
                .WithMessage("At least one available ingredient is required.");

            RuleFor(command => command.Recipes)
                .NotNull()
                .NotEmpty()
                .WithMessage("At least one recipe is required.");

            RuleForEach(command => command.AvailableIngredients)
                .SetValidator(new AvailableIngredientInputValidator());

            RuleForEach(command => command.Recipes)
                .SetValidator(new RecipeInputValidator());
        }
    }

    internal sealed class AvailableIngredientInputValidator
        : AbstractValidator<AvailableIngredientInput>
    {
        public AvailableIngredientInputValidator()
        {
            RuleFor(ingredient => ingredient.Name)
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(ingredient => ingredient.Quantity)
                .GreaterThanOrEqualTo(0);
        }
    }

    internal sealed class RecipeInputValidator
        : AbstractValidator<RecipeInput>
    {
        public RecipeInputValidator()
        {
            RuleFor(recipe => recipe.Name)
                .NotEmpty()
                .MaximumLength(200);

            RuleFor(recipe => recipe.Servings)
                .GreaterThan(0);

            RuleFor(recipe => recipe.Ingredients)
                .NotNull()
                .NotEmpty()
                .WithMessage("A recipe must contain at least one ingredient.");

            RuleForEach(recipe => recipe.Ingredients)
                .SetValidator(new IngredientRequirementInputValidator());
        }
    }

    internal sealed class IngredientRequirementInputValidator
        : AbstractValidator<IngredientRequirementInput>
    {
        public IngredientRequirementInputValidator()
        {
            RuleFor(ingredient => ingredient.Name)
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(ingredient => ingredient.Quantity)
                .GreaterThan(0);
        }
    }
}
