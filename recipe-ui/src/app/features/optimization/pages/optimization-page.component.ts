import {
  ChangeDetectionStrategy,
  Component,
  inject,
  signal
} from '@angular/core';

import {
  KeyValuePipe
} from '@angular/common';

import {
  FormArray,
  FormControl,
  FormGroup,
  ReactiveFormsModule,
  Validators
} from '@angular/forms';

import {
  OptimizationService
} from '../services/optimization.service';

import {
    OptimizeRecipesRequest,
  OptimizeRecipesResponse
} from '../models/optimization.models';

import {
  IngredientRowComponent
} from '../components/ingredient-row/ingredient-row.component';

import {
  RecipeCardComponent
} from '../components/recipe-card/recipe-card.component';

import {
  OptimizationExampleService
} from '../services/optimization-example.service';

import { duplicateNamesValidator } from '../validators/optimization-form.validators';

import {
  getApiErrorMessage
} from '../../../core/http/api-error-message';

import {
  DestroyRef,
} from '@angular/core';

import {
  takeUntilDestroyed
} from '@angular/core/rxjs-interop';

type IngredientForm = FormGroup<{
  name: FormControl<string>;
  quantity: FormControl<number>;
}>;

type RequirementForm = FormGroup<{
  name: FormControl<string>;
  quantity: FormControl<number>;
}>;

type RecipeForm = FormGroup<{
  name: FormControl<string>;
  servings: FormControl<number>;
  ingredients: FormArray<RequirementForm>;
}>;

@Component({
  selector: 'app-optimization-page',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    KeyValuePipe,
    IngredientRowComponent,
    RecipeCardComponent
  ],
  templateUrl: './optimization-page.component.html',
  styleUrl: './optimization-page.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class OptimizationPageComponent {
  private readonly optimizationService =
    inject(OptimizationService);

  readonly loading = signal(false);

  readonly error = signal<string | null>(null);

  readonly result =
    signal<OptimizeRecipesResponse | null>(null);

  readonly availableIngredients =
  new FormArray<IngredientForm>(
    [],
    {
      validators: [
        duplicateNamesValidator()
      ]
    }
  );

  readonly recipes =
    new FormArray<RecipeForm>([]);

    readonly selectedExampleId = signal('');

    readonly form = new FormGroup({
    availableIngredients: this.availableIngredients,
    recipes: this.recipes
    });

    private readonly exampleService = inject(OptimizationExampleService);

    readonly examples = this.exampleService.examples;

    private readonly destroyRef = inject(DestroyRef);

  constructor() {
    this.addIngredient();
    this.addRecipe();
  }

  addIngredient(): void {
    this.availableIngredients.push(
      this.createIngredient()
    );
  }

  removeIngredient(index: number): void {
    this.availableIngredients.removeAt(index);
    this.availableIngredients.updateValueAndValidity();
    }

  addRecipe(): void {
    this.recipes.push(
      this.createRecipe()
    );
  }

  removeRecipe(index: number): void {
    this.recipes.removeAt(index);
  }

  addRequirement(recipeIndex: number): void {
    this.recipes
      .at(recipeIndex)
      .controls
      .ingredients
      .push(this.createRequirement());
  }

  removeRequirement(
    recipeIndex: number,
    requirementIndex: number
    ): void {
    const ingredients =
        this.recipes
        .at(recipeIndex)
        .controls
        .ingredients;

    ingredients.removeAt(requirementIndex);
    ingredients.updateValueAndValidity();
    }

  optimize(): void {
        if (this.form.invalid) {
            this.form.markAllAsTouched();

            return;
        }

        this.loading.set(true);
        this.error.set(null);
        this.result.set(null);

        const request = this.form.getRawValue();

        this.optimizationService
            .optimize(request)
            .pipe(
                takeUntilDestroyed(this.destroyRef)
            )
            .subscribe({
            next: result => {
                this.result.set(result);
                this.loading.set(false);
            },

            error: error => {
                console.error(
                    'Recipe optimization failed:',
                    error
                );

                this.error.set(
                    getApiErrorMessage(error)
                );

                this.loading.set(false);
            }
            });
        }

    loadExample(id: string): void {
        if (!id) {
            return;
        }

        const example =
            this.exampleService.getById(id);

        if (!example) {
            return;
        }

        this.loadPayload(example.payload);

        this.selectedExampleId.set(id);

        this.result.set(null);
        this.error.set(null);
    }

    private loadPayload(
        payload: OptimizeRecipesRequest
        ): void {
        this.availableIngredients.clear();
        this.recipes.clear();

        for (
            const ingredient of payload.availableIngredients
        ) {
            this.availableIngredients.push(
            this.createIngredient(
                ingredient.name,
                ingredient.quantity
            )
            );
        }

        for (
            const recipe of payload.recipes
        ) {
            const recipeForm =
            this.createRecipe(
                recipe.name,
                recipe.servings
            );

            for (
            const ingredient of recipe.ingredients
            ) {
            recipeForm.controls.ingredients.push(
                this.createRequirement(
                ingredient.name,
                ingredient.quantity
                )
            );
            }

            this.recipes.push(recipeForm);
        }
        }
    

  private createIngredient(
    name = '',
    quantity = 0
    ): IngredientForm {
        return new FormGroup({
            name: new FormControl(
                name,
                {
                nonNullable: true,
                validators: [
                    Validators.required,
                    Validators.maxLength(100)
                ]
                }
            ),

            quantity: new FormControl(
                quantity,
                {
                nonNullable: true,
                validators: [
                    Validators.min(0)
                ]
                }
            )
            });
  }

  private createRequirement(
    name = '',
    quantity = 1
    ): RequirementForm {
        return new FormGroup({
            name: new FormControl(
                name,
                {
                nonNullable: true,
                validators: [
                    Validators.required,
                    Validators.maxLength(100)
                ]
                }
            ),

            quantity: new FormControl(
                quantity,
                {
                nonNullable: true,
                validators: [
                    Validators.min(1)
                ]
                }
            )
            });
  }

  private createRecipe(
        name = '',
        servings = 1
        ): RecipeForm {
        return new FormGroup({
            name: new FormControl(
            name,
            {
                nonNullable: true,
                validators: [
                Validators.required,
                Validators.maxLength(200)
                ]
            }
            ),

            servings: new FormControl(
            servings,
            {
                nonNullable: true,
                validators: [
                Validators.required,
                Validators.min(1)
                ]
            }
            ),

            ingredients: new FormArray<RequirementForm>(
            [],
            {
                validators: [
                Validators.minLength(1),
                duplicateNamesValidator()
                ]
            }
            )
        });
        }
  resetForm(): void {
        this.availableIngredients.clear();
        this.recipes.clear();

        this.addIngredient();
        this.addRecipe();

        this.selectedExampleId.set('');

        this.result.set(null);
        this.error.set(null);
    }
    
}