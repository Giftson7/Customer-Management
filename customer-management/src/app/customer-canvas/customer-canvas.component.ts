import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { AVATAR_COLORS} from '../constants/customer-constant';
import { Client, CustomerDto } from '../service-proxies/serviceProxy';
import { firstValueFrom } from 'rxjs';
import { AuthService } from '../services/auth.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-customer-canvas',
  templateUrl: './customer-canvas.component.html',
  styleUrl: './customer-canvas.component.css',
  standalone: false,
})
export class CustomerCanvasComponent implements OnInit{
  // Customer data
  customers: CustomerDto[] = [];
  filteredCustomers: CustomerDto[] = [];
  
  // Filter and search
  selectedCustomerType: string = 'all';
  searchTerm: string = '';
  
  // Sort functionality
  showSortDropdown: boolean = false;
  currentSort: string = 'name-asc';
  
  // Form functionality
  showAddForm: boolean = false;
  customerForm: FormGroup;
  
  // Snackbar
  showSnackbar: boolean = false;
  snackbarMessage: string = '';
  


  constructor(private readonly fb: FormBuilder,
    private readonly serviceProxy: Client,
    private readonly authService: AuthService,
    private readonly router: Router,
  ) {
    this.customerForm = this.fb.group({
      title: ['', Validators.required],
      firstName: ['', Validators.required],
      lastName: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      phone: ['', [Validators.required, Validators.pattern(/^[\+]?[1-9][\d]{0,15}$/)]],
      customerType: ['', Validators.required]
    });
  }

  ngOnInit(): void {
    this.initializeSampleData().then(() => {
      this.filterCustomers();
    });
  }

  private async initializeSampleData() {
    const customers = await firstValueFrom(this.serviceProxy.getCustomers());
    this.customers = customers;
  }

  filterCustomers() {
    let filtered = [...this.customers];

    // Filter by customer type
    if (this.selectedCustomerType !== 'all') {
      filtered = filtered.filter(customer => customer.customerType === this.selectedCustomerType);
    }

    // Filter by search term
    if (this.searchTerm.trim()) {
      const searchLower = this.searchTerm.toLowerCase();
      filtered = filtered.filter(customer => 
        customer?.firstName?.toLowerCase().includes(searchLower) ||
        customer?.lastName?.toLowerCase().includes(searchLower) ||
        customer?.email?.toLowerCase().includes(searchLower)
      );
    }

    // Apply current sort
    this.sortCustomers(this.currentSort, filtered);
  }

  sortCustomers(sortType: string, customersToSort: CustomerDto[] = this.filteredCustomers) {
    this.currentSort = sortType;
    this.showSortDropdown = false;

    const sorted = [...customersToSort].sort((a, b) => {
      switch (sortType) {
        case 'name-asc':
          return (a.firstName + ' ' + a.lastName).localeCompare(b.firstName + ' ' + b.lastName);
        case 'name-desc':
          return (b.firstName + ' ' + b.lastName).localeCompare(a.firstName + ' ' + a.lastName);
        case 'email-asc':
          return (a?.email ?? '').localeCompare(b?.email ?? '');
        case 'email-desc':
          return (b?.email ?? '').localeCompare(a?.email ?? '');
        default:
          return 0;
      }
    });

    this.filteredCustomers = sorted;
  }

  toggleSortDropdown() {
    this.showSortDropdown = !this.showSortDropdown;
  }

  openAddCustomerForm() {
    this.showAddForm = true;
    this.customerForm.reset();
  }

  closeAddCustomerForm() {
    this.showAddForm = false;
    this.customerForm.reset();
  }

  onSubmit() {
    if (this.customerForm.valid) {
      const formValue = this.customerForm.value;
      this.addCustomer(formValue);
    } else {
      this.markFormGroupTouched();
    }
  }

  private markFormGroupTouched() {
    Object.keys(this.customerForm.controls).forEach(key => {
      const control = this.customerForm.get(key);
      control?.markAsTouched();
    });
  }

  viewCustomer(customer: CustomerDto) {
    console.log('View customer:', customer);
  }

  editCustomer(customer: CustomerDto) {
    console.log('Edit customer:', customer);
  }

  deleteCustomer(customer: CustomerDto) {
    if (confirm(`Are you sure you want to delete ${customer.firstName} ${customer.lastName}?`)) {
      this.customers = this.customers.filter(c => c.id !== customer.id);
      this.filterCustomers();
      this.showSnackbarMessage('Customer Deleted!');
    }
  }

  private showSnackbarMessage(message: string) {
    this.snackbarMessage = message;
    this.showSnackbar = true;
    
    setTimeout(() => {
      this.showSnackbar = false;
    }, 3000);
  }

  private async addCustomer(formValue: any) {
    let newCustomer: CustomerDto = {
        title: formValue.title,
        firstName: formValue.firstName,
        lastName: formValue.lastName,
        email: formValue.email,
        phone: formValue.phone,
        customerType: formValue.customerType,
        status: 'active',
        avatarColor: AVATAR_COLORS[Math.floor(Math.random() * AVATAR_COLORS.length)]
      } as CustomerDto;

      newCustomer = await firstValueFrom(this.serviceProxy.addCustomers(newCustomer));
      this.customers.unshift(newCustomer);
      this.closeAddCustomerForm();
      this.filterCustomers();
      this.showSnackbarMessage('Customer Added!');
  }

  logOut() {
    const confirmed = confirm('Are you sure you want to log out?');
    if (confirmed) {
      this.authService.logout();
      this.router.navigate(['/login']);
    }
  }

}
