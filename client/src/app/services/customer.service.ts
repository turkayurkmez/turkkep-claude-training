import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Customer } from '../models/customer';
import { PagedResult } from '../models/paged-result';

@Injectable({ providedIn: 'root' })
export class CustomerService {
  private readonly _http = inject(HttpClient);
  private readonly _apiUrl = 'http://localhost:5015';

  getCustomers(page = 1, pageSize = 10): Observable<PagedResult<Customer>> {
    return this._http.get<PagedResult<Customer>>(
      `${this._apiUrl}/customers?page=${page}&pageSize=${pageSize}`
    );
  }
}
