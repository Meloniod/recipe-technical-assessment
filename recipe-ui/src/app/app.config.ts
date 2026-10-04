import {
  ApplicationConfig
} from '@angular/core';

import {
  provideHttpClient
} from '@angular/common/http';

import {
  API_CONFIG
} from './core/config/api.config';

import {
  environment
} from '../environments/environment';

export const appConfig: ApplicationConfig = {
  providers: [
    provideHttpClient(),

    {
      provide: API_CONFIG,
      useValue: {
        baseUrl: environment.apiUrl
      }
    }
  ]
};