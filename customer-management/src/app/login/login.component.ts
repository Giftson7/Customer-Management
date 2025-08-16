import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../services/auth.service';
import { Client, GoogleLoginRequest, LoginRequest, LoginResponse } from '../service-proxies/serviceProxy';
import { HttpClient } from '@angular/common/http';

declare const google: any;


@Component({
  selector: 'login',
  templateUrl: './login.component.html',
  styleUrl: './login.component.css',
  standalone: false,
})
export class LoginComponent implements OnInit {
  loginData: LoginRequest = new LoginRequest();
  isLoading = false;
  errorMessage = '';
  isRegisterMode = false;


  constructor(
    private readonly authService: AuthService,
    private readonly router: Router,
    private readonly http: HttpClient,
    private readonly serviceProxy: Client
  ) { }


  ngOnInit(): void {
    const checkGoogle = setInterval(() => {
      if (typeof google !== 'undefined' && google?.accounts?.id) {
        google.accounts.id.renderButton(
          document.getElementById("g_id_signin"),
          { type: "standard", shape: "circle", theme: "outline", size: "large" }
        );
        clearInterval(checkGoogle);
      }
    }, 100);

    this.initializeGoogleSignIn();
  }


  private initializeGoogleSignIn(): void {
    // Google will call this function after user signs in
    (window as any).handleCredentialResponse = (response: any) => {

      this.serviceProxy.google({ idToken: response.credential } as GoogleLoginRequest)
        .subscribe({
          next: (res: any) => {
            if (res.success && res.token) {
              // Save JWT & user info
              this.authService.saveAuthData({
                token: res.token
              } as LoginResponse);

              // Redirect to customer page
              this.router.navigate(['/customer']);
            } else {
              this.errorMessage = res.message ?? 'Google login failed.';
            }
          },
          error: (err) => {
            console.error("Google login error", err);
            this.errorMessage = 'Google login failed.';
          }
        });
    };
  }


  onSubmit(): void {
    this.isLoading = true;
    this.errorMessage = '';

    const request = this.isRegisterMode
      ? this.authService.register(this.loginData)
      : this.authService.login(this.loginData);

    request.subscribe({
      next: (response: LoginResponse) => {
        if (response.success) {
          this.authService.saveAuthData(response);
          this.router.navigate(['/customer']);
        } else {
          this.errorMessage = response.message ?? '';
        }
      },
      error: (error) => {
        this.errorMessage = 'An error occurred. Please try again.';
        console.error('Auth error:', error);
      },
      complete: () => {
        this.isLoading = false;
      }
    });
  }

  toggleMode(): void {
    this.isRegisterMode = !this.isRegisterMode;
    this.errorMessage = '';
  }
}
