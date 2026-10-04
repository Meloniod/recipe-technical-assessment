import {
  OptimizeRecipesRequest
} from '../models/optimization.models';

export interface OptimizationExample {
  id: string;
  name: string;
  description: string;
  payload: OptimizeRecipesRequest;
}

export const OPTIMIZATION_EXAMPLES:
  OptimizationExample[] = [

  {
    id: 'classic-sandwich',
    name: 'Classic Sandwich Lunch',
    description:
      'A simple lunch scenario with competing sandwich and salad recipes.',
    payload: {
      availableIngredients: [
        { name: 'Bread', quantity: 10 },
        { name: 'Lettuce', quantity: 5 },
        { name: 'Chicken', quantity: 4 }
      ],
      recipes: [
        {
          name: 'Chicken Sandwich',
          servings: 2,
          ingredients: [
            { name: 'Bread', quantity: 2 },
            { name: 'Lettuce', quantity: 1 },
            { name: 'Chicken', quantity: 1 }
          ]
        },
        {
          name: 'Chicken Salad',
          servings: 1,
          ingredients: [
            { name: 'Lettuce', quantity: 2 },
            { name: 'Chicken', quantity: 1 }
          ]
        }
      ]
    }
  },

  {
    id: 'breakfast-mix',
    name: 'Breakfast Mix',
    description:
      'Several breakfast recipes competing for eggs, bread and other ingredients.',
    payload: {
      availableIngredients: [
        { name: 'Eggs', quantity: 12 },
        { name: 'Bread', quantity: 8 },
        { name: 'Cheese', quantity: 4 },
        { name: 'Tomato', quantity: 6 }
      ],
      recipes: [
        {
          name: 'Egg Sandwich',
          servings: 2,
          ingredients: [
            { name: 'Eggs', quantity: 2 },
            { name: 'Bread', quantity: 2 }
          ]
        },
        {
          name: 'Cheese Omelette',
          servings: 1,
          ingredients: [
            { name: 'Eggs', quantity: 2 },
            { name: 'Cheese', quantity: 1 }
          ]
        },
        {
          name: 'Tomato Toast',
          servings: 1,
          ingredients: [
            { name: 'Bread', quantity: 1 },
            { name: 'Tomato', quantity: 2 }
          ]
        }
      ]
    }
  },

  {
    id: 'family-dinner',
    name: 'Family Dinner',
    description:
      'Larger recipes designed to feed several people per preparation.',
    payload: {
      availableIngredients: [
        { name: 'Rice', quantity: 10 },
        { name: 'Chicken', quantity: 6 },
        { name: 'Carrots', quantity: 8 },
        { name: 'Onions', quantity: 5 }
      ],
      recipes: [
        {
          name: 'Chicken Rice',
          servings: 4,
          ingredients: [
            { name: 'Rice', quantity: 3 },
            { name: 'Chicken', quantity: 2 },
            { name: 'Onions', quantity: 1 }
          ]
        },
        {
          name: 'Chicken Stew',
          servings: 3,
          ingredients: [
            { name: 'Chicken', quantity: 2 },
            { name: 'Carrots', quantity: 3 },
            { name: 'Onions', quantity: 2 }
          ]
        }
      ]
    }
  },

  {
    id: 'leftovers',
    name: 'Lots of Leftovers',
    description:
      'A scenario where the optimal solution can leave ingredients unused.',
    payload: {
      availableIngredients: [
        { name: 'Bread', quantity: 20 },
        { name: 'Lettuce', quantity: 3 },
        { name: 'Cheese', quantity: 10 },
        { name: 'Tomato', quantity: 2 }
      ],
      recipes: [
        {
          name: 'Cheese Sandwich',
          servings: 2,
          ingredients: [
            { name: 'Bread', quantity: 2 },
            { name: 'Cheese', quantity: 1 }
          ]
        },
        {
          name: 'Garden Sandwich',
          servings: 1,
          ingredients: [
            { name: 'Bread', quantity: 2 },
            { name: 'Lettuce', quantity: 1 },
            { name: 'Tomato', quantity: 1 }
          ]
        }
      ]
    }
  },

  {
    id: 'competitive-combination',
    name: 'Competitive Combination',
    description:
      'Multiple recipes compete for overlapping ingredients.',
    payload: {
      availableIngredients: [
        { name: 'Potatoes', quantity: 12 },
        { name: 'Chicken', quantity: 6 },
        { name: 'Rice', quantity: 10 },
        { name: 'Carrots', quantity: 6 }
      ],
      recipes: [
        {
          name: 'Chicken Potato Meal',
          servings: 4,
          ingredients: [
            { name: 'Potatoes', quantity: 3 },
            { name: 'Chicken', quantity: 2 }
          ]
        },
        {
          name: 'Chicken Rice Meal',
          servings: 3,
          ingredients: [
            { name: 'Rice', quantity: 2 },
            { name: 'Chicken', quantity: 1 }
          ]
        },
        {
          name: 'Vegetable Rice',
          servings: 2,
          ingredients: [
            { name: 'Rice', quantity: 2 },
            { name: 'Carrots', quantity: 2 }
          ]
        }
      ]
    }
  }
];