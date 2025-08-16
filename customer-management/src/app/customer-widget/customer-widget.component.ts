import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CustomerDto } from '../service-proxies/serviceProxy';

@Component({
  selector: 'app-customer-widget',
  templateUrl: './customer-widget.component.html',
  styleUrls: ['./customer-widget.component.css'],
  standalone: false
})
export class CustomerWidgetComponent {
  @Input() customer!: CustomerDto;
  @Output() view = new EventEmitter<CustomerDto>();
  @Output() edit = new EventEmitter<CustomerDto>();
  @Output() delete = new EventEmitter<CustomerDto>();

  get fullName(): string {
    return `${this.customer.firstName} ${this.customer.lastName}`;
  }

  get initials(): string {
    return `${this.customer?.firstName?.charAt(0)}${this.customer?.lastName?.charAt(0)}`.toUpperCase();
  }

  onView() {
    this.view.emit(this.customer);
  }

  onEdit() {
    this.edit.emit(this.customer);
  }

  onDelete() {
    this.delete.emit(this.customer);
  }
} 