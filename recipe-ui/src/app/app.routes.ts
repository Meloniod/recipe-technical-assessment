import {
  Routes
} from '@angular/router';

export const routes: Routes = [
  {
    path: '',
    redirectTo: 'optimization',
    pathMatch: 'full'
  },

  {
    path: 'optimization',
    loadComponent: () =>
      import(
        './features/optimization/pages/optimization-page.component'
      ).then(
        module => module.OptimizationPageComponent
      )
  },

  {
    path: '**',
    redirectTo: 'optimization'
  }
];