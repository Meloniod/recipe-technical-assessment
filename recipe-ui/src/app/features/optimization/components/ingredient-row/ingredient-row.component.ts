import {
  ChangeDetectionStrategy,
  Component,
  EventEmitter,
  Input,
  Output
} from '@angular/core';

import {
  FormControl,
  ReactiveFormsModule
} from '@angular/forms';

@Component({
  selector: 'app-ingredient-row',
  standalone: true,
  imports: [
    ReactiveFormsModule
  ],
  templateUrl: './ingredient-row.component.html',
  styleUrl: './ingredient-row.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class IngredientRowComponent {
  @Input({ required: true })
  nameControl!: FormControl<string>;

  @Input({ required: true })
  quantityControl!: FormControl<number>;

  @Output()
  readonly remove = new EventEmitter<void>();
}