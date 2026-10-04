import {
  ChangeDetectionStrategy,
  Component,
  inject,
  signal
} from '@angular/core';

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

@Component({
  selector: 'app-optimization-page',
  standalone: true,
  imports: [
    ReactiveFormsModule
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

  readonly form = new FormGroup({
    availableIngredients: new FormArray([]),
    recipes: new FormArray([])
  });

  optimize(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();

      return;
    }

    this.loading.set(true);
    this.error.set(null);
    this.result.set(null);

    this.optimizationService
      .optimize(this.form.getRawValue())
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
}