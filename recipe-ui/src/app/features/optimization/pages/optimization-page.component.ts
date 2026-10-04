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
  OptimizeRecipesResponse
} from '../models/optimization.models';

import {
  IngredientRowComponent
} from '../components/ingredient-row/ingredient-row.component';

import {
  RecipeCardComponent
} from '../components/recipe-card/recipe-card.component';

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
    new FormArray<IngredientForm>([]);

  readonly recipes =
    new FormArray<RecipeForm>([]);

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
    this.recipes
      .at(recipeIndex)
      .controls
      .ingredients
      .removeAt(requirementIndex);
  }

  optimize(): void {
    if (
      this.availableIngredients.invalid ||
      this.recipes.invalid
    ) {
      this.availableIngredients.markAllAsTouched();
      this.recipes.markAllAsTouched();

      return;
    }

    this.loading.set(true);
    this.error.set(null);
    this.result.set(null);

    const request = {
      availableIngredients:
        this.availableIngredients.getRawValue(),

      recipes:
        this.recipes.getRawValue()
    };

    this.optimizationService
      .optimize(request)
      .subscribe({
        next: result => {
          this.result.set(result);
          this.loading.set(false);
        },

        error: () => {
          this.error.set(
            'Unable to calculate the optimal combination. Please try again.'
          );

          this.loading.set(false);
        }
      });
  }

  private createIngredient(): IngredientForm {
    return new FormGroup({
      name: new FormControl(
        '',
        {
          nonNullable: true,
          validators: [
            Validators.required,
            Validators.maxLength(100)
          ]
        }
      ),

      quantity: new FormControl(
        0,
        {
          nonNullable: true,
          validators: [
            Validators.min(0)
          ]
        }
      )
    });
  }

  private createRequirement(): RequirementForm {
    return new FormGroup({
      name: new FormControl(
        '',
        {
          nonNullable: true,
          validators: [
            Validators.required,
            Validators.maxLength(100)
          ]
        }
      ),

      quantity: new FormControl(
        1,
        {
          nonNullable: true,
          validators: [
            Validators.min(1)
          ]
        }
      )
    });
  }

  private createRecipe(): RecipeForm {
    return new FormGroup({
      name: new FormControl(
        '',
        {
          nonNullable: true,
          validators: [
            Validators.required,
            Validators.maxLength(200)
          ]
        }
      ),

      servings: new FormControl(
        1,
        {
          nonNullable: true,
          validators: [
            Validators.min(1)
          ]
        }
      ),

      ingredients: new FormArray<RequirementForm>([])
    });
  }
}