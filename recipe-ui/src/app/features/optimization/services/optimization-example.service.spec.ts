import {
  TestBed
} from '@angular/core/testing';

import {
  OptimizationExampleService
} from './optimization-example.service';

describe(
  'OptimizationExampleService',
  () => {
    let service: OptimizationExampleService;

    beforeEach(() => {
      TestBed.configureTestingModule({});

      service =
        TestBed.inject(
          OptimizationExampleService
        );
    });

    it(
      'should provide optimization examples',
      () => {
        expect(
          service.examples.length
        ).toBe(5);
      }
    );

    it(
      'should find an example by id',
      () => {
        const example =
          service.getById(
            'classic-sandwich'
          );

        expect(example).toBeDefined();

        expect(
          example?.name
        ).toBe(
          'Classic Sandwich Lunch'
        );
      }
    );

    it(
      'should return undefined for an unknown example',
      () => {
        const example =
          service.getById(
            'does-not-exist'
          );

        expect(example).toBeUndefined();
      }
    );
  }
);