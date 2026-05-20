import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Customer } from '../models/customer';

@Injectable({ providedIn: 'root' })
export class CustomerService {
  private readonly _http = inject(HttpClient);
  private readonly _apiUrl = 'http://localhost:5015';

  getCustomers(): Observable<Customer[]> {
    return this._http.get<Customer[]>(`${this._apiUrl}/customers`);
  }
}
