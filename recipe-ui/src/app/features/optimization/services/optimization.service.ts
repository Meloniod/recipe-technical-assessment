import {
  inject,
  Injectable
} from '@angular/core';

import {
  HttpClient
} from '@angular/common/http';

import {
  Observable
} from 'rxjs';

import {
  API_CONFIG
} from '../../../core/config/api.config';

import {
  OptimizeRecipesRequest,
  OptimizeRecipesResponse
} from '../models/optimization.models';

@Injectable({
  providedIn: 'root'
})
export class OptimizationService {
  private readonly http = inject(HttpClient);

  private readonly apiConfig = inject(API_CONFIG);

  private readonly endpoint =
    `${this.apiConfig.baseUrl}/api/v1/recipes/optimize`;

  optimize(
    request: OptimizeRecipesRequest
  ): Observable<OptimizeRecipesResponse> {
    console.log("Endpoint URL:", this.endpoint);
    console.log("OptimizationService.optimize called with request:", request);
    return this.http.post<OptimizeRecipesResponse>(
      this.endpoint,
      request
    );
  }
}