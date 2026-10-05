import {
  HttpTestingController,
  provideHttpClientTesting
} from '@angular/common/http/testing';

import {
  TestBed
} from '@angular/core/testing';

import {
  provideHttpClient
} from '@angular/common/http';

import {
  OptimizationService
} from './optimization.service';

import {
  OptimizeRecipesRequest
} from '../models/optimization.models';

import {
  API_CONFIG
} from '../../../core/config/api.config';

describe(
  'OptimizationService',
  () => {
    let service: OptimizationService;
    let httpMock: HttpTestingController;

    const apiUrl =
      'https://localhost:7105';

    beforeEach(() => {
      TestBed.configureTestingModule({
        providers: [
          provideHttpClient(),
          provideHttpClientTesting(),

          {
            provide: API_CONFIG,
            useValue: {
              baseUrl: apiUrl
            }
          }
        ]
      });

      service =
        TestBed.inject(
          OptimizationService
        );

      httpMock =
        TestBed.inject(
          HttpTestingController
        );
    });

    afterEach(() => {
      httpMock.verify();
    });

    it(
      'should POST the optimization request',
      () => {
        const request:
          OptimizeRecipesRequest = {
          availableIngredients: [
            {
              name: 'Bread',
              quantity: 4
            }
          ],

          recipes: [
            {
              name: 'Sandwich',
              servings: 2,
              ingredients: [
                {
                  name: 'Bread',
                  quantity: 2
                }
              ]
            }
          ]
        };

        service
          .optimize(request)
          .subscribe();

        const httpRequest =
          httpMock.expectOne(
            `${apiUrl}/api/v1/recipes/optimize`
          );

        expect(
          httpRequest.request.method
        ).toBe('POST');

        expect(
          httpRequest.request.body
        ).toEqual(request);

        httpRequest.flush({
          allocations: [
            {
              recipeName: 'Sandwich',
              quantity: 2,
              peopleFed: 4
            }
          ],
          peopleFed: 4,
          unusedIngredients: {
            Bread: 0
          }
        });
      }
    );
  }
);