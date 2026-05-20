import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { rxResource } from '@angular/core/rxjs-interop';
import { Customer } from '../../models/customer';
import { CustomerService } from '../../services/customer.service';
import { CustomerCardComponent } from './customer-card';

@Component({
  selector: 'app-customer-list',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [CustomerCardComponent],
  template: `
    @if (customersResource.isLoading()) {
      <p>Yükleniyor...</p>
    } @else if (customersResource.error()) {
      <p>Veriler yüklenemedi.</p>
    } @else {
      <ul>
        @for (customer of customersResource.value(); track customer.id) {
          <li>
            <app-customer-card [customer]="customer" />
          </li>
        }
      </ul>
    }
  `,
})
export class CustomerListComponent {
  private readonly _customerService = inject(CustomerService);

  readonly customersResource = rxResource<Customer[], unknown>({
    stream: () => this._customerService.getCustomers(),
  });
}
