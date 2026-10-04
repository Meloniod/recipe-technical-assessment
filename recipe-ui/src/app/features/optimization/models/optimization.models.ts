export interface AvailableIngredient {
  name: string;
  quantity: number;
}

export interface IngredientRequirement {
  name: string;
  quantity: number;
}

export interface Recipe {
  name: string;
  servings: number;
  ingredients: IngredientRequirement[];
}

export interface OptimizeRecipesRequest {
  availableIngredients: AvailableIngredient[];
  recipes: Recipe[];
}

export interface RecipeAllocation {
  recipeName: string;
  quantity: number;
  peopleFed: number;
}

export interface OptimizeRecipesResponse {
  allocations: RecipeAllocation[];
  peopleFed: number;
  unusedIngredients: Record<string, number>;
}