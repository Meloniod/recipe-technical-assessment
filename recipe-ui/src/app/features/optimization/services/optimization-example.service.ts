import {
  Injectable
} from '@angular/core';

import {
  OPTIMIZATION_EXAMPLES,
  OptimizationExample
} from '../data/optimization-examples';

@Injectable({
  providedIn: 'root'
})
export class OptimizationExampleService {

  readonly examples =
    OPTIMIZATION_EXAMPLES;

  getById(
    id: string
  ): OptimizationExample | undefined {
    return this.examples.find(
      example => example.id === id
    );
  }
}