import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LoginComponent } from './login/login.component';
import { AuthGuard } from './auth-guard/auth.guard';
import { CustomerCanvasComponent } from './customer-canvas/customer-canvas.component';

const routes: Routes = [
  { path: 'login', component: LoginComponent },
  { path: 'customer', component: CustomerCanvasComponent, canActivate: [AuthGuard] },
  { path: '', redirectTo: '/login', pathMatch: 'full' },
  { path: '**', redirectTo: '/login' }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { } 