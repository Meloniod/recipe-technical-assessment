import {
  ComponentFixture,
  TestBed
} from '@angular/core/testing';

import {
  provideHttpClient
} from '@angular/common/http';

import {
    HttpTestingController,
  provideHttpClientTesting
} from '@angular/common/http/testing';

import {
  OptimizationPageComponent
} from './optimization-page.component';

import {
  API_CONFIG
} from '../../../core/config/api.config';

describe(
  'OptimizationPageComponent',
  () => {
    let component:
      OptimizationPageComponent;

    let fixture:
      ComponentFixture<
        OptimizationPageComponent
      >;
    
    let httpMock: HttpTestingController;

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            imports: [
            OptimizationPageComponent
            ],
            providers: [
            provideHttpClient(),
            provideHttpClientTesting(),
            {
                provide: API_CONFIG,
                useValue: {
                baseUrl: 'https://localhost:7105'
                }
            }
            ]
        }).compileComponents();

        fixture = TestBed.createComponent(
            OptimizationPageComponent
        );

        component = fixture.componentInstance;

        httpMock = TestBed.inject(
            HttpTestingController
        );

        fixture.detectChanges();
    });

    it(
      'should create',
      () => {
        expect(component).toBeTruthy();
      }
    );

    it(
      'should start with one ingredient and one recipe',
      () => {
        expect(
          component.availableIngredients.length
        ).toBe(1);

        expect(
          component.recipes.length
        ).toBe(1);
      }
    );

    it(
        'should add an available ingredient',
        () => {
            component.addIngredient();

            expect(
            component.availableIngredients.length
            ).toBe(2);
        }
        );

        it(
        'should remove an available ingredient',
        () => {
            component.removeIngredient(0);

            expect(
            component.availableIngredients.length
            ).toBe(0);
        }
    );

    it(
        'should add a recipe',
        () => {
            component.addRecipe();

            expect(
            component.recipes.length
            ).toBe(2);
        }
        );

        it(
        'should remove a recipe',
        () => {
            component.removeRecipe(0);

            expect(
            component.recipes.length
            ).toBe(0);
        }
    );

    it(
        'should reject duplicate available ingredients',
        () => {
            const first =
            component.availableIngredients.at(0);

            first.controls.name.setValue('Bread');

            component.addIngredient();

            const second =
            component.availableIngredients.at(1);

            second.controls.name.setValue('bread');

            component.availableIngredients.markAllAsTouched();

            expect(
            component.availableIngredients.hasError(
                'duplicateNames'
            )
            ).toBeTruthy();
        }
    );

    it(
        'should reject a recipe without ingredients',
        () => {
            const recipe =
            component.recipes.at(0);

            recipe.controls.ingredients.markAsTouched();

            expect(
                recipe.controls.ingredients.hasError(
                    'requiredArray'
                )
            ).toBeTruthy();
        }
    );

    it(
        'should reject duplicate recipe ingredients',
        () => {
            component.addRequirement(0);
            component.addRequirement(0);

            const ingredients =
            component.recipes
                .at(0)
                .controls
                .ingredients;

            ingredients.at(0)
            .controls.name
            .setValue('Bread');

            ingredients.at(1)
            .controls.name
            .setValue('bread');

            ingredients.markAsTouched();

            expect(
            ingredients.hasError(
                'duplicateNames'
            )
            ).toBeTruthy();
        }
    );

    it(
        'should reset the form',
        () => {
            component.addIngredient();
            component.addRecipe();
            component.addRequirement(0);

            component.resetForm();

            expect(
            component.availableIngredients.length
            ).toBe(1);

            expect(
            component.recipes.length
            ).toBe(1);

            expect(
            component.recipes
                .at(0)
                .controls
                .ingredients
                .length
            ).toBe(0);

            expect(
            component.selectedExampleId()
            ).toBe('');

            expect(
            component.result()
            ).toBeNull();

            expect(
            component.error()
            ).toBeNull();
        }
    );

    it(
        'should load an example into the form',
        () => {
            component.loadExample(
            'classic-sandwich'
            );

            expect(
            component.selectedExampleId()
            ).toBe(
            'classic-sandwich'
            );

            expect(
            component.availableIngredients.length
            ).toBe(3);

            expect(
            component.recipes.length
            ).toBe(2);

            expect(
            component.availableIngredients
                .at(0)
                .controls
                .name
                .value
            ).toBe('Bread');

            expect(
            component.availableIngredients
                .at(0)
                .controls
                .quantity
                .value
            ).toBe(10);

            expect(
            component.recipes
                .at(0)
                .controls
                .name
                .value
            ).toBe(
            'Chicken Sandwich'
            );

            expect(
            component.recipes
                .at(0)
                .controls
                .ingredients
                .length
            ).toBe(3);
        }
    );

    it(
        'should not submit an invalid form',
        () => {
            component.optimize();

            httpMock.expectNone(
            'https://localhost:7105/api/v1/recipes/optimize'
            );
        }
    );

    it(
        'should display the optimization result',
        () => {
            const ingredient =
            component.availableIngredients.at(0);

            ingredient.controls.name
            .setValue('Bread');

            ingredient.controls.quantity
            .setValue(4);

            const recipe =
            component.recipes.at(0);

            recipe.controls.name
            .setValue('Sandwich');

            recipe.controls.servings
            .setValue(2);

            component.addRequirement(0);

            const requirement =
            recipe.controls.ingredients.at(0);

            requirement.controls.name
            .setValue('Bread');

            requirement.controls.quantity
            .setValue(2);

            component.optimize();

            const request =
            httpMock.expectOne(
                'https://localhost:7105/api/v1/recipes/optimize'
            );

            expect(
            request.request.method
            ).toBe('POST');

            request.flush({
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

            expect(
            component.loading()
            ).toBeFalsy();

            expect(
            component.result()?.peopleFed
            ).toBe(4);
        }
    );

    it(
        'should display a useful error when the API is unavailable',
        () => {
            const ingredient =
            component.availableIngredients.at(0);

            ingredient.controls.name
            .setValue('Bread');

            ingredient.controls.quantity
            .setValue(4);

            const recipe =
            component.recipes.at(0);

            recipe.controls.name
            .setValue('Sandwich');

            recipe.controls.servings
            .setValue(2);

            component.addRequirement(0);

            const requirement =
            recipe.controls.ingredients.at(0);

            requirement.controls.name
            .setValue('Bread');

            requirement.controls.quantity
            .setValue(2);

            component.optimize();

            const request =
            httpMock.expectOne(
                'https://localhost:7105/api/v1/recipes/optimize'
            );

            request.flush(
            null,
            {
                status: 0,
                statusText: 'Unknown Error'
            }
            );

            expect(
            component.loading()
            ).toBeFalsy();

            expect(
            component.error()
            ).toBe(
            'Unable to connect to the API. Please check that the backend is running.'
            );
        }
    );
  }
);