import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: '',
    loadComponent: () =>
      import('./features/customers/customer-list').then(m => m.CustomerListComponent),
  },
];
