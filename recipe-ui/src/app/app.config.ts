import {
  ApplicationConfig
} from '@angular/core';

import {
  provideRouter
} from '@angular/router';

import {
  provideHttpClient
} from '@angular/common/http';

import {
  API_CONFIG
} from './core/config/api.config';

import {
  routes
} from './app.routes';

import {
  environment
} from '../environments/environment';

export const appConfig: ApplicationConfig = {
  providers: [
    provideRouter(routes),

    provideHttpClient(),

    {
      provide: API_CONFIG,
      useValue: {
        baseUrl: environment.apiUrl
      }
    }
  ]
};