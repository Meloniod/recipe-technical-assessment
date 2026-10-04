import {
  ChangeDetectionStrategy,
  Component,
  EventEmitter,
  Input,
  Output
} from '@angular/core';

import {
  FormArray,
  FormControl,
  FormGroup,
  ReactiveFormsModule
} from '@angular/forms';

@Component({
  selector: 'app-recipe-card',
  standalone: true,
  imports: [
    ReactiveFormsModule
  ],
  templateUrl: './recipe-card.component.html',
  styleUrl: './recipe-card.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class RecipeCardComponent {
  @Input({ required: true })
  recipe!: FormGroup;

  @Input({ required: true })
  requirements!: FormArray;

  @Output()
  readonly remove = new EventEmitter<void>();

  @Output()
  readonly addRequirement = new EventEmitter<void>();

  @Output()
  readonly removeRequirement =
    new EventEmitter<number>();

  get nameControl(): FormControl<string> {
    return this.recipe.get('name') as FormControl<string>;
  }

  get servingsControl(): FormControl<number> {
    return this.recipe.get(
      'servings'
    ) as FormControl<number>;
  }

  getRequirementNameControl(
    index: number
  ): FormControl<string> {
    return this.requirements
      .at(index)
      .get('name') as FormControl<string>;
  }

  getRequirementQuantityControl(
    index: number
  ): FormControl<number> {
    return this.requirements
      .at(index)
      .get('quantity') as FormControl<number>;
  }
}