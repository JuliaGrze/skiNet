import { Component, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { MatFormField, MatLabel } from '@angular/material/form-field';
import {MatCardModule} from '@angular/material/card';
import { MatInput } from '@angular/material/input';
import { MatButton } from '@angular/material/button';
import { AccountService } from '../../../core/services/account.service';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
  selector: 'app-login',
  imports: [
    ReactiveFormsModule,
    MatCardModule,
    MatFormField,
    MatInput,
    MatLabel,
    MatButton
  ],
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss'
})
export class LoginComponent {
  private fb = inject(FormBuilder)
  private accountService = inject(AccountService)
  private router = inject(Router)
  private activateRoute = inject(ActivatedRoute)
  returnUrl = '/shop'

  constructor(){
    const url = this.activateRoute.snapshot.queryParams['returnUrl']
    if(url)
      this.returnUrl = url
  }

  //Create loginForm - FormGroup with 2 fields
  loginForm = this.fb.group({
    email: [''],
    password: ['']
  })

  onSubmit(){
    this.accountService.login(this.loginForm.value).subscribe({
      next: () => {
        this.accountService.getUserInfo().subscribe(() => {
        this.router.navigateByUrl(this.returnUrl);
        })
      }
    })
  }
}
