import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { rxResource } from '@angular/core/rxjs-interop';
import { PagedResult } from '../../models/paged-result';
import { Customer } from '../../models/customer';
import { CustomerService } from '../../services/customer.service';
import { CustomerCardComponent } from './customer-card';

@Component({
  selector: 'app-customer-list',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [CustomerCardComponent],
  template: `
    <div class="container py-4">
      <h1 class="fw-semibold text-primary mb-4">Müşteriler</h1>

      @if (customersResource.isLoading()) {
        <div class="d-flex align-items-center gap-2 text-secondary" role="status" aria-live="polite">
          <div class="spinner-border spinner-border-sm text-primary" aria-hidden="true"></div>
          <span>Yükleniyor...</span>
        </div>
      } @else if (customersResource.error()) {
        <div class="alert alert-danger" role="alert">
          Veriler yüklenemedi. Lütfen daha sonra tekrar deneyin.
        </div>
      } @else if (customersResource.value()!.items.length === 0) {
        <div class="text-center text-secondary py-5" role="status">
          <p class="mb-0">Henüz müşteri yok.</p>
        </div>
      } @else {
        <ul class="list-unstyled d-flex flex-column gap-2" aria-label="Müşteri listesi">
          @for (customer of customersResource.value()!.items; track customer.id) {
            <li>
              <app-customer-card [customer]="customer" />
            </li>
          }
        </ul>
      }
    </div>
  `,
})
export class CustomerListComponent {
  private readonly _customerService = inject(CustomerService);

  readonly customersResource = rxResource<PagedResult<Customer>, unknown>({
    stream: () => this._customerService.getCustomers(),
  });
}
