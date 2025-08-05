// Localização: src/app/auth/register/register.ts

import { Component, OnInit, OnDestroy } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule, AbstractControl, ValidatorFn } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { Router, RouterLink } from '@angular/router';
import { Subscription } from 'rxjs';
import { finalize } from 'rxjs/operators';
import { MatIconModule } from '@angular/material/icon';

import { AuthService } from '@auth/auth.service';
import { RegisterRequestDto } from '@auth/models/register-request.model';
import { LoginResponse } from '@auth/models/login-response.model';
import { ApiResponse, isPreservedCollection } from '@models/common/api-response.model';
import { NotificationDto } from '@models/common/notification.model';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatCardModule,
    MatInputModule,
    MatButtonModule,
    MatFormFieldModule,
    MatProgressSpinnerModule,
    MatSnackBarModule,
    RouterLink,
    MatIconModule
  ],
  templateUrl: './register.html',
  styleUrls: ['./register.scss']
})
export class RegisterComponent implements OnInit, OnDestroy {
  registerForm!: FormGroup;
  isLoading: boolean = false;
  private authSubscription: Subscription | null = null;

  notifications: NotificationDto[] = [];
  errorMessage: string | null = null;
  hidePassword = true;
  hideConfirmPassword = true;

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private snackBar: MatSnackBar,
    private router: Router
  ) {
    console.log('[RegisterComponent] Constructor.');
  }

  ngOnInit(): void {
    console.log('[RegisterComponent] ngOnInit: Inicializando formulário de registro.');
    this.registerForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      apelido: ['', Validators.required],
      cpf: ['', [Validators.required, Validators.pattern(/^\d{11}$/)]],
      celular: ['', [Validators.required, Validators.pattern(/^\d{10,11}$/)]],
      password: ['', [Validators.required, Validators.minLength(6)]],
      confirmPassword: ['', Validators.required],
      // Removidos scheme e host
      sendConfirmationEmail: [false]
    }, { validators: this.passwordMatchValidator });
    
    this.authSubscription = this.authService.isAuthenticated$.subscribe(isAuthenticated => {
      if (isAuthenticated) {
        console.log('[RegisterComponent] Usuário já autenticado, redirecionando para o dashboard.');
        this.router.navigate(['/dashboard']);
      }
    });
  }

  ngOnDestroy(): void {
    console.log('[RegisterComponent] ngOnDestroy: Desinscrevendo do authSubscription.');
    if (this.authSubscription) {
      this.authSubscription.unsubscribe();
    }
  }

  passwordMatchValidator: ValidatorFn = (control: AbstractControl): { [key: string]: boolean } | null => {
    const password = control.get('password');
    const confirmPassword = control.get('confirmPassword');

    if (password && confirmPassword && password.value !== confirmPassword.value) {
      confirmPassword.setErrors({ passwordMismatch: true });
      return { passwordMismatch: true };
    } else if (confirmPassword && confirmPassword.hasError('passwordMismatch') && password.value === confirmPassword.value) {
      confirmPassword.setErrors(null);
    }
    return null;
  };

  togglePasswordVisibility(): void {
    this.hidePassword = !this.hidePassword;
  }

  toggleConfirmPasswordVisibility(): void {
    this.hideConfirmPassword = !this.hideConfirmPassword;
  }

  onSubmit(): void {
    console.log('[RegisterComponent] onSubmit chamado.');
    this.notifications = [];
    this.errorMessage = null;

    if (this.registerForm.invalid) {
      this.showSnackBar('Por favor, preencha todos os campos corretamente, incluindo a confirmação de senha.', 'Fechar', 'error');
      this.registerForm.markAllAsTouched();
      return;
    }

    this.isLoading = true;
    const registerRequest: RegisterRequestDto = this.registerForm.value;

    this.authService.register(registerRequest).pipe(
      finalize(() => {
        this.isLoading = false;
        console.log('[RegisterComponent] Finalize do registro. isLoading agora é:', this.isLoading);
      })
    ).subscribe({
      next: (response: ApiResponse<LoginResponse>) => {
        const loginResponseData = isPreservedCollection<LoginResponse>(response.data) ? response.data.$values[0] : response.data;

        if (response.success && loginResponseData?.loginSucesso) {
          this.showSnackBar('Registro realizado! Faça login para continuar.', 'Fechar', 'success');
          console.log('[RegisterComponent] Registro bem-sucedido. Redirecionando para /login.');
          this.router.navigate(['/login']);
        } else {
          let messageToDisplay = response.message || '';
          if (response.errors) {
            messageToDisplay += (messageToDisplay ? '; ' : '') + (Array.isArray(response.errors) ? response.errors.join('; ') : response.errors);
          }
          if (loginResponseData?.erros) {
              messageToDisplay += (messageToDisplay ? '; ' : '') + (typeof loginResponseData.erros === 'string' ? loginResponseData.erros : (Array.isArray(loginResponseData.erros) ? loginResponseData.erros.join('; ') : ''));
          }
          if (!messageToDisplay) {
            messageToDisplay = 'Erro ao registrar.';
          }
          this.handleApiResponseError(messageToDisplay, 'Erro ao registrar.');
        }
      },
      error: (errorResponse: any) => {
        this.isLoading = false;
        let message = 'Erro de conexão ou dados inválidos.';
        if (errorResponse && errorResponse.message) {
          message = errorResponse.message;
        }
        this.errorMessage = message;
        this.showSnackBar(message, 'Fechar', 'error');
        console.error('[RegisterComponent] Erro no processo de registro:', errorResponse);
      }
    });
  }

  private handleApiResponseError(backendErrors: any, defaultMessage: string): void {
    let messageToDisplay = defaultMessage;
    if (backendErrors) {
      if (typeof backendErrors === 'string') {
        messageToDisplay = backendErrors;
      } else if (Array.isArray(backendErrors)) {
        messageToDisplay = backendErrors.join('; ');
      } else if (typeof backendErrors === 'object') {
        const errorMessages = Object.values(backendErrors)
          .flat()
          .filter(err => typeof err === 'string')
          .join('; ');
        if (errorMessages) {
          messageToDisplay = errorMessages;
        }
      }
    }
    this.errorMessage = messageToDisplay;
    this.showSnackBar(messageToDisplay, 'Fechar', 'error');
    this.notifications.push({ mensagem: messageToDisplay, tipo: 'Erro', codigo: '' });
  }

  private showSnackBar(message: string, action: string = 'Fechar', type: 'success' | 'error' | 'warning' | 'info' = 'info'): void {
    let panelClass: string[] = [];
    if (type === 'success') {
      panelClass = ['snackbar-success'];
    } else if (type === 'error') {
      panelClass = ['snackbar-error'];
    } else if (type === 'warning') {
      panelClass = ['snackbar-warning'];
    } else if (type === 'info') {
      panelClass = ['snackbar-info'];
    }

    this.snackBar.open(message, action, {
      duration: 5000,
      horizontalPosition: 'center',
      verticalPosition: 'bottom',
      panelClass: panelClass
    });
  }
}
