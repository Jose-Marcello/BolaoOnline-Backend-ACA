// Localização: src/app/auth/components/login/login.component.ts

import { Component, OnInit, OnDestroy } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { Subscription } from 'rxjs';
import { finalize } from 'rxjs/operators';

import { AuthService } from '@auth/auth.service';
import { LoginRequestDto } from '@auth/models/login-request.model';
import { LoginResponse } from '@auth/models/login-response.model';
import { ApiResponse, isPreservedCollection } from '@models/common/api-response.model';
import { NotificationDto } from '@models/common/notification.model';

@Component({
  selector: 'app-login',
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
    MatIconModule // <<-- ADICIONADO AQUI -->>
  ],
  templateUrl: './login.html',
  styleUrls: ['./login.scss']
})
export class LoginComponent implements OnInit, OnDestroy {
  loginForm!: FormGroup;
  isLoading: boolean = false;
  private authSubscription: Subscription | null = null;

  // <<-- PROPRIEDADES ADICIONADAS AQUI -->>
  notifications: NotificationDto[] = [];
  errorMessage: string | null = null;
  hidePassword = true;

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private snackBar: MatSnackBar,
    private router: Router
  ) {
    console.log('[LoginComponent] Constructor.');
  }

  ngOnInit(): void {
    console.log('[LoginComponent] ngOnInit: Inicializando formulário de login.');
    this.loginForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]]
    });

    this.authSubscription = this.authService.isAuthenticated$.subscribe(isAuthenticated => {
      if (isAuthenticated) {
        console.log('[LoginComponent] Usuário já autenticado, redirecionando para o dashboard.');
        this.router.navigate(['/dashboard']);
      }
    });
  }

  ngOnDestroy(): void {
    console.log('[LoginComponent] ngOnDestroy: Desinscrevendo do authSubscription.');
    if (this.authSubscription) {
      this.authSubscription.unsubscribe();
    }
  }

  // <<-- MÉTODO ADICIONADO AQUI -->>
  togglePasswordVisibility(): void {
    this.hidePassword = !this.hidePassword;
  }

  onSubmit(): void {
    console.log('[LoginComponent] onSubmit chamado.');
    this.notifications = []; // Limpa notificações anteriores
    this.errorMessage = null; // Limpa mensagens de erro anteriores

    if (this.loginForm.invalid) {
      this.showSnackBar('Por favor, preencha o e-mail e a senha corretamente.', 'Fechar', 'error');
      this.loginForm.markAllAsTouched();
      return;
    }

    this.isLoading = true;
    const loginRequest: LoginRequestDto = this.loginForm.value;

    this.authService.login(loginRequest).pipe(
      finalize(() => {
        this.isLoading = false;
        console.log('[LoginComponent] Finalize do login. isLoading agora é:', this.isLoading);
      })
    ).subscribe({
      next: (response: ApiResponse<LoginResponse>) => {
        const loginResponseData = isPreservedCollection<LoginResponse>(response.data) ? response.data.$values[0] : response.data;

        if (response.success && loginResponseData?.loginSucesso) {
          console.log('[LoginComponent] Login bem-sucedido na resposta da API, aguardando redirecionamento do AuthService.');
          this.showSnackBar('Login realizado com sucesso!', 'Fechar', 'success');
          // O redirecionamento é feito no AuthService.tap
        } else {
          let errorMessage = response.message || '';
          if (response.errors) {
            errorMessage += (errorMessage ? '; ' : '') + (Array.isArray(response.errors) ? response.errors.join('; ') : response.errors);
          }
          if (loginResponseData?.erros) {
              errorMessage += (errorMessage ? '; ' : '') + (typeof loginResponseData.erros === 'string' ? loginResponseData.erros : (Array.isArray(loginResponseData.erros) ? loginResponseData.erros.join('; ') : ''));
          }
          if (!errorMessage) {
            errorMessage = 'Credenciais inválidas. Tente novamente.';
          }
          this.errorMessage = errorMessage; // Define a mensagem de erro para ser exibida no HTML
          this.showSnackBar(errorMessage, 'Fechar', 'error');
          this.notifications.push({ mensagem: errorMessage, tipo: 'Erro', codigo: '' }); // Adiciona como notificação também
          console.error('[LoginComponent] Falha no login (resposta da API):', response);
        }
      },
      error: (err) => {
        const errorMessage = err.message || 'Erro de conexão. Verifique sua rede.';
        this.errorMessage = errorMessage; // Define a mensagem de erro para ser exibida no HTML
        this.showSnackBar(errorMessage, 'Fechar', 'error');
        this.notifications.push({ mensagem: errorMessage, tipo: 'Erro', codigo: '' }); // Adiciona como notificação também
        console.error('[LoginComponent] Erro no processo de login:', err);
      }
    });
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
