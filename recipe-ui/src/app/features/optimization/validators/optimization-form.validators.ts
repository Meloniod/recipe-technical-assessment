import {
  AbstractControl,
  ValidationErrors,
  ValidatorFn
} from '@angular/forms';

export function duplicateNamesValidator(
  caseInsensitive = true
): ValidatorFn {
  return (
    control: AbstractControl
  ): ValidationErrors | null => {
    const controls = control.value as Array<{
      name?: string;
    }> | null;

    if (!controls || controls.length === 0) {
      return null;
    }

    const names = controls
      .map(item => item.name?.trim() ?? '')
      .filter(Boolean)
      .map(name =>
        caseInsensitive
          ? name.toLowerCase()
          : name
      );

    const duplicates =
      names.filter(
        (name, index) =>
          names.indexOf(name) !== index
      );

    return duplicates.length > 0
      ? {
          duplicateNames: true
        }
      : null;
  };
}