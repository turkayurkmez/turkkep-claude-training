import { ChangeDetectionStrategy, Component, input } from '@angular/core';
import { Customer } from '../../models/customer';

@Component({
  selector: 'app-customer-card',
  changeDetection: ChangeDetectionStrategy.OnPush,
  styles: [`
    .customer-card {
      transition: box-shadow 0.15s ease-in-out;
    }
    .customer-card:hover {
      box-shadow: 0 4px 12px rgba(0, 0, 0, 0.08) !important;
    }
  `],
  template: `
    <div class="customer-card card border-0 shadow-sm px-3 py-2"
         role="article"
         [attr.aria-label]="customer().name + ' müşteri kartı'">
      <div class="card-body p-0 d-flex flex-column gap-1">
        <span class="fw-semibold text-primary">{{ customer().name }}</span>
        <span class="text-secondary small">{{ customer().email }}</span>
      </div>
    </div>
  `,
})
export class CustomerCardComponent {
  readonly customer = input.required<Customer>();
}
