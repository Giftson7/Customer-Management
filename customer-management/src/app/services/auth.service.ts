// services/auth.service.ts
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import {Client, LoginRequest, LoginResponse} from '../service-proxies/serviceProxy'; 

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private readonly apiUrl = 'http://localhost:6001/api/auth';

  constructor(private readonly http: HttpClient,
    private readonly serviceProxy: Client
  ) { }

  login(credentials: LoginRequest): Observable<LoginResponse> {
    return this.serviceProxy.login(credentials);
  }

  register(credentials: LoginRequest): Observable<LoginResponse> {
    return this.serviceProxy.register(credentials);
  }

  isLoggedIn(): boolean {
    return !!localStorage.getItem('token');
  }

  getToken(): string | null {
    return localStorage.getItem('token');
  }

  logout(): void {
    localStorage.removeItem('token');
    localStorage.removeItem('user');
  }

  saveAuthData(response: LoginResponse): void {
    if (response.token) {
      localStorage.setItem('token', response.token);
    }
    if (response.user) {
      localStorage.setItem('user', JSON.stringify(response.user));
    }
  }
}