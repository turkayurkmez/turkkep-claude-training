import { ChangeDetectionStrategy, Component, input } from '@angular/core';
import { Customer } from '../../models/customer';

@Component({
  selector: 'app-customer-card',
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <div class="customer-card">
      <span>{{ customer().name }}</span>
      <span>{{ customer().email }}</span>
    </div>
  `,
})
export class CustomerCardComponent {
  readonly customer = input.required<Customer>();
}
