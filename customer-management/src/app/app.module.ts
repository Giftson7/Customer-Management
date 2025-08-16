import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { AppComponent } from './app.component';
import { CustomerWidgetComponent } from './customer-widget/customer-widget.component';
import { AppRoutingModule } from './app-routing.module';
import { LoginComponent } from './login/login.component';
import { CustomerCanvasComponent } from './customer-canvas/customer-canvas.component';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { API_BASE_URL, Client } from './service-proxies/serviceProxy';
import { authInterceptor } from './interceptor/auth-interceptor';

@NgModule({
  declarations: [
    AppComponent,
    CustomerWidgetComponent,
    LoginComponent,
    CustomerCanvasComponent
  ],
  imports: [
    BrowserModule,
    BrowserAnimationsModule,
    FormsModule,
    ReactiveFormsModule,
    AppRoutingModule,
  ],
  providers: [provideHttpClient(withInterceptors([authInterceptor])), Client,
    { provide: API_BASE_URL, useValue: 'http://localhost:6001' } 
  ],
  bootstrap: [AppComponent]
})
export class AppModule { } 