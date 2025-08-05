// Localização: src/app/aposta-rodada/aposta-rodada-form/aposta-rodada-form.component.ts

import { Component, OnInit, OnDestroy, LOCALE_ID } from '@angular/core';
import { CommonModule, DatePipe, registerLocaleData } from '@angular/common';
import localePt from '@angular/common/locales/pt';
import { FormBuilder, FormGroup, FormArray, Validators, ReactiveFormsModule, AbstractControl } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatSelectModule } from '@angular/material/select';

import { Subscription, of } from 'rxjs';
import { switchMap, map, tap, finalize } from 'rxjs/operators';
import { ApostadorService } from '@services/apostador.service';

import { ApostaService } from '@services/aposta.service';
import { CampeonatoService } from '@services/campeonato.service';
import { RodadaService } from '@services/rodada.service'; // <<-- NOVO: Importar RodadaService -->>
import { SalvarApostaRequestDto } from '@models/aposta/salvar-aposta-request-dto.model';
import { NotificationDto } from '@models/common/notification.model';
import { RodadaDto, StatusRodada } from '@models/rodada/rodada-dto.model';
import { CampeonatoDto } from '@models/campeonato/campeonato-dto.model';
import { ApostaJogoVisualizacaoDto } from '@models/aposta/aposta-jogo-visualizacao-dto.model';
import { ApostaRodadaDto } from '@models/aposta/aposta-rodada-dto.model';
import { PalpiteDto } from '@models/palpite/palpite-dto.model';
import { environment } from '@environments/environment';
import { ApiResponse, isPreservedCollection, PreservedCollection } from '@models/common/api-response.model';
import { AuthService } from '@auth/auth.service';
import { ApostadorCampeonatoDto } from '@models/apostador-campeonato/apostador-campeonato-dto.model';
import { ApostadorDto } from '@models/apostador/apostador-dto.model';

registerLocaleData(localePt, 'pt-BR');

const APOSTA_STATUS_NAO_ENVIADA = 0;
const APOSTA_STATUS_ENVIADA = 1;

@Component({
  selector: 'app-aposta-rodada-form',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    RouterLink,
    MatSnackBarModule,
    MatProgressSpinnerModule,
    MatCardModule,
    MatIconModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatSelectModule
  ],
  providers: [
    DatePipe,
    { provide: LOCALE_ID, useValue: 'pt-BR' }
  ],
  templateUrl: './aposta-rodada-form.component.html',
  styleUrls: ['./aposta-rodada-form.component.scss']
})
export class ApostarRodadaFormComponent implements OnInit, OnDestroy {
  campeonatoId: string | null = null;
  apostadorCampeonatoId: string | null = null;
  rodadaId: string | null = null;
  apostaRodadaId: string | null = null;

  rodadasEmAposta: RodadaDto[] = [];
  rodadaSelecionada: RodadaDto | null = null;

  apostasUsuarioRodada: ApostaRodadaDto[] = [];

  apostaAtual: ApostaRodadaDto = this.getEmptyApostaRodadaDto();

  jogosDaApostaAtual: ApostaJogoVisualizacaoDto[] = [];

  isLoading: boolean = true;
  isSaving: boolean = false;
  errorMessage: string | null = null;

  apostaForm!: FormGroup;
  baseUrlImagens: string = environment.imagesUrl;

  hasNotifications: boolean = false;
  notifications: NotificationDto[] = [];

  readonly APOSTA_STATUS_NAO_ENVIADA = APOSTA_STATUS_NAO_ENVIADA;
  readonly APOSTA_STATUS_ENVIADA = APOSTA_STATUS_ENVIADA;
  public StatusRodada = StatusRodada;

  private subscriptions: Subscription = new Subscription();
  apostador: ApostadorDto | null = null; // Mantido para outras lógicas se necessário

  constructor(
    private fb: FormBuilder,
    private apostaService: ApostaService,
    private campeonatoService: CampeonatoService,
    private apostadorService: ApostadorService,
    private rodadaService: RodadaService, // <<-- NOVO: Injetar RodadaService -->>
    private authService: AuthService,
    private snackBar: MatSnackBar,
    private route: ActivatedRoute,
    private router: Router,
    private datePipe: DatePipe
  ) {
    this.apostaForm = this.fb.group({
      palpites: this.fb.array([])
    });
  }

  ngOnInit(): void {
    this.subscriptions.add(
      this.route.paramMap.pipe(
        tap(params => {
          this.campeonatoId = params.get('campeonatoId');
          this.rodadaId = params.get('rodadaId'); // Pode ser nulo
          this.apostaRodadaId = params.get('apostaRodadaId');
          
          console.log('[DEBUG] ngOnInit: Parâmetros da URL (paramMap) - CampeonatoId:', this.campeonatoId, 'RodadaId:', this.rodadaId, 'ApostaRodadaId:', this.apostaRodadaId);
        }),
        switchMap(() => this.route.queryParamMap),
        tap(queryParams => {
          const apostadorCampeonatoIdFromQuery = queryParams.get('apostadorCampeonatoId');
          this.apostadorCampeonatoId = apostadorCampeonatoIdFromQuery === 'null' ? null : apostadorCampeonatoIdFromQuery;
          console.log('[DEBUG] ngOnInit: QueryParams - ApostadorCampeonatoId (da query):', this.apostadorCampeonatoId);
        }),
        switchMap(() => {
          if (!this.campeonatoId) {
            this.errorMessage = 'ID do campeonato não fornecido na URL.';
            this.isLoading = false;
            return of(null);
          }
          return this.apostadorService.getDadosApostador().pipe(
            tap(apostadorResponse => {
              if (apostadorResponse.success && apostadorResponse.data) {
                this.apostador = isPreservedCollection<ApostadorDto>(apostadorResponse.data) 
                                ? (apostadorResponse.data.$values && apostadorResponse.data.$values.length > 0 ? apostadorResponse.data.$values[0] : null)
                                : apostadorResponse.data as ApostadorDto;
                console.log('[DEBUG] ngOnInit: Apostador carregado (para referência):', this.apostador);
              } else {
                console.warn('[WARN] ngOnInit: Falha ao carregar dados do apostador para referência.');
                this.apostador = null;
              }
            }),
            map(() => ({
              campeonatoId: this.campeonatoId,
              rodadaId: this.rodadaId,
              apostadorCampeonatoId: this.apostadorCampeonatoId,
              apostaRodadaId: this.apostaRodadaId
            }))
          );
        })
      ).subscribe(data => {
        if (data && data.campeonatoId) {
          this.loadAllIntegratedData(data.campeonatoId, data.rodadaId, data.apostadorCampeonatoId, data.apostaRodadaId);
        } else {
          this.errorMessage = this.errorMessage || 'Não foi possível carregar informações essenciais para a aposta.';
          this.isLoading = false;
        }
      })
    );
  }

  ngOnDestroy(): void {
    this.subscriptions.unsubscribe();
  }

  get palpites(): FormArray<FormGroup> {
    return this.apostaForm.get('palpites') as FormArray<FormGroup>;
  }

  private getEmptyApostaRodadaDto(): ApostaRodadaDto {
    return {
      id: '',
      apostadorCampeonatoId: '', 
      rodadaId: '', 
      identificadorAposta: 'Nova Aposta Avulsa',
      statusAposta: APOSTA_STATUS_NAO_ENVIADA,
      ehApostaCampeonato: false,
      ehApostaIsolada: true,
      custoPagoApostaRodada: 0,
      pontuacaoTotalRodada: 0,
      enviada: false, 
      numJogosApostados: 0, 
      apostadorCampeonato: null, 
      dataHoraSubmissao: null
    };
  }

  getStatusRodadaText(status: StatusRodada): string {
    switch (status) {
      case StatusRodada.EmAposta: return 'Em Aposta';
      case StatusRodada.Corrente: return 'Corrente';
      case StatusRodada.Finalizada: return 'Finalizada';
      case StatusRodada.NaoIniciada: return 'Não Iniciada';
      default: return 'Desconhecido';
    }
  }

  private async loadAllIntegratedData(campeonatoId: string, initialRodadaId: string | null, apostadorCampeonatoId: string | null, initialApostaRodadaId: string | null): Promise<void> {
    this.isLoading = true;
    this.hasNotifications = false;
    this.notifications = [];
    this.errorMessage = null;

    console.log('[DEBUG] loadAllIntegratedData - CampeonatoId:', campeonatoId, 'RodadaId:', initialRodadaId, 'ApostadorCampeonatoId (param):', apostadorCampeonatoId, 'ApostaRodadaId:', initialApostaRodadaId);


    try {
      // <<-- ATUALIZADO: Usando RodadaService -->>
      const rodadasResponse = await this.rodadaService.getRodadasEmAposta(campeonatoId).toPromise();
      if (rodadasResponse?.success && rodadasResponse.data) {
        this.rodadasEmAposta = isPreservedCollection<RodadaDto>(rodadasResponse.data)
                                 ? rodadasResponse.data.$values
                                 : Array.isArray(rodadasResponse.data) ? rodadasResponse.data : [];
        console.log('[DEBUG] Rodadas em aposta carregadas:', this.rodadasEmAposta);

        if (this.rodadasEmAposta.length === 0) {
          this.errorMessage = 'Nenhuma rodada disponível para apostar neste campeonato.';
          this.isLoading = false;
          return;
        }

        this.rodadaSelecionada = this.rodadasEmAposta.find(r => r.id === initialRodadaId) || null;
        if (!this.rodadaSelecionada) {
          this.rodadaSelecionada = this.rodadasEmAposta.length > 0 ? this.rodadasEmAposta[0] : null;
        }
        
        if (this.rodadaSelecionada) {
          this.rodadaId = this.rodadaSelecionada.id;
          await this.loadApostasUsuarioRodada(this.rodadaId, apostadorCampeonatoId, initialApostaRodadaId);
        } else {
          this.errorMessage = 'Rodada inicial não encontrada e nenhuma rodada alternativa disponível para apostar.';
          this.isLoading = false;
          return;
        }

        console.log('[DEBUG] Rodada selecionada para exibição:', this.rodadaSelecionada);

      } else {
        this.handleApiResponseNotifications(rodadasResponse!);
        this.errorMessage = this.errorMessage || 'Falha ao carregar rodadas em aposta.';
      }
    } catch (error: any) {
      console.error('Erro geral ao carregar dados da aposta:', error);
      this.errorMessage = error.message || 'Erro ao carregar dados da aposta.';
    } finally {
      this.isLoading = false;
    }
  }

  async loadApostasUsuarioRodada(rodadaId: string, apostadorCampeonatoId: string | null, initialApostaRodadaId: string | null): Promise<void> {
    this.isLoading = true;
    this.errorMessage = null;
    this.apostasUsuarioRodada = [];

    console.log('[DEBUG] loadApostasUsuarioRodada - RodadaId:', rodadaId, 'ApostadorCampeonatoId (param):', apostadorCampeonatoId, 'InitialApostaRodadaId:', initialApostaRodadaId);

    if (!rodadaId || !apostadorCampeonatoId) {
      this.errorMessage = 'IDs de rodada ou apostador do campeonato faltando para carregar apostas. Exibindo estado inicial.';
      this.isLoading = false;
      console.warn('[WARN] loadApostasUsuarioRodada - IDs de rodada ou apostador do campeonato faltando. Não buscando apostas do usuário. RodadaId:', rodadaId, 'ApostadorCampeonatoId:', apostadorCampeonatoId);
      this.apostasUsuarioRodada = [];
      this.apostaAtual = this.getEmptyApostaRodadaDto();
      this.jogosDaApostaAtual = [];
      this.palpites.clear();
      this.buildForm();
      return;
    }

    try {
      const apostasResponse = await this.apostaService.getApostasPorRodadaEUsuario(rodadaId, apostadorCampeonatoId).toPromise();
      if (apostasResponse?.success && apostasResponse.data) {
        this.apostasUsuarioRodada = isPreservedCollection<ApostaRodadaDto>(apostasResponse.data)
                                         ? apostasResponse.data.$values
                                         : Array.isArray(apostasResponse.data) ? apostasResponse.data : [];
        console.log('[DEBUG] Apostas do usuário para a rodada (getApostasPorRodadaEUsuario):', this.apostasUsuarioRodada);

        if (initialApostaRodadaId) {
          this.apostaAtual = this.apostasUsuarioRodada.find(a => a.id === initialApostaRodadaId) || this.getEmptyApostaRodadaDto();
        } else {
          this.apostaAtual = this.apostasUsuarioRodada.find(a => a.ehApostaCampeonato) || (this.apostasUsuarioRodada.length > 0 ? this.apostasUsuarioRodada[0] : this.getEmptyApostaRodadaDto());
        }
        console.log('[DEBUG] Aposta Atual Selecionada (após getApostasPorRodadaEUsuario):', this.apostaAtual);

      } else {
        this.handleApiResponseNotifications(apostasResponse!);
        this.errorMessage = this.errorMessage || 'Falha ao carregar suas apostas para esta rodada.';
        this.apostaAtual = this.getEmptyApostaRodadaDto();
      }
    } catch (error: any) {
      console.error('Erro ao carregar apostas do usuário para a rodada:', error);
      this.errorMessage = error.message || 'Erro ao carregar suas apostas para esta rodada.';
      this.apostaAtual = this.getEmptyApostaRodadaDto();
    } finally {
      await this.loadJogosDaApostaAtual(this.rodadaId!, this.apostaAtual.id);
      this.isLoading = false;
    }
  }

  async loadJogosDaApostaAtual(rodadaId: string, apostaRodadaId: string): Promise<void> {
    this.isLoading = true;
    this.errorMessage = null;
    this.jogosDaApostaAtual = [];
    this.palpites.clear();

    console.log(`[DEBUG] loadJogosDaApostaAtual - rodadaId: ${rodadaId}, apostaRodadaId (param): ${apostaRodadaId}`);
    console.log(`[DEBUG] loadJogosDaApostaAtual - this.apostadorCampeonatoId (component property, da rota): ${this.apostadorCampeonatoId}`);
    console.log(`[DEBUG] loadJogosDaApostaAtual - this.apostaAtual.id: ${this.apostaAtual.id}, this.apostaAtual.apostadorCampeonatoId: ${this.apostaAtual.apostadorCampeonatoId}`);

    const apostadorCampeonatoIdToUse = this.apostadorCampeonatoId; 

    if (!rodadaId || !apostadorCampeonatoIdToUse) {
      this.errorMessage = 'IDs de rodada ou apostador do campeonato faltando para carregar jogos. Exibindo estado inicial.';
      this.isLoading = false;
      console.warn('[WARN] loadJogosDaApostaAtual - IDs de rodada ou apostador do campeonato faltando. Não buscando jogos para aposta. RodadaId:', rodadaId, 'apostadorCampeonatoIdToUse:', apostadorCampeonatoIdToUse);
      this.jogosDaApostaAtual = [];
      this.palpites.clear();
      this.buildForm();
      return;
    }

    try {
      const jogosResponse = await this.apostaService.getApostasParaEdicao(rodadaId, apostadorCampeonatoIdToUse).toPromise();
      
      if (jogosResponse?.success && jogosResponse.data) {
        this.jogosDaApostaAtual = isPreservedCollection<ApostaJogoVisualizacaoDto>(jogosResponse.data)
                                 ? jogosResponse.data.$values
                                 : Array.isArray(jogosResponse.data) ? jogosResponse.data : [];

        console.log('[DEBUG] Dados recebidos de getApostasParaEdicao (ApostaJogoVisualizacaoDto):', this.jogosDaApostaAtual);

        if (apostaRodadaId === '') {
          this.jogosDaApostaAtual.forEach(jogo => {
            jogo.placarApostaCasa = null; 
            jogo.placarApostaVisita = null; 
          });
          console.log('[DEBUG] Jogos carregados para nova aposta (palpites limpos no frontend):', this.jogosDaApostaAtual);
        } else {
          console.log('[DEBUG] Jogos da aposta atual carregados:', this.jogosDaApostaAtual);
        }
        
        this.buildForm();
      } else {
        this.handleApiResponseNotifications(jogosResponse!);
        this.errorMessage = this.errorMessage || 'Falha ao carregar os jogos da aposta.';
      }
    } catch (error: any) {
      console.error('Erro ao carregar jogos da aposta atual:', error);
      this.errorMessage = error.message || 'Erro ao carregar os jogos da aposta.';
    } finally {
      this.isLoading = false;
    }
  }

  onClickCriarNovaAposta(): void {
    this.apostaAtual = this.getEmptyApostaRodadaDto();
    if (this.rodadaId && this.apostadorCampeonatoId) {
      this.loadJogosDaApostaAtual(this.rodadaId, ''); 
    } else {
      this.showNotification('IDs de rodada ou apostador do campeonato não disponíveis para criar nova aposta.', 'warning');
    }
  }

  async onRodadaSelected(selectedRodadaId: string): Promise<void> {
    this.isLoading = true;
    this.errorMessage = null;
    this.rodadaSelecionada = this.rodadasEmAposta.find(r => r.id === selectedRodadaId) || null;

    if (this.rodadaSelecionada) {
      this.rodadaId = this.rodadaSelecionada.id;
      this.apostaAtual = this.getEmptyApostaRodadaDto(); 
      await this.loadApostasUsuarioRodada(this.rodadaId, this.apostadorCampeonatoId, null);
    } else {
      this.showNotification('Rodada selecionada não encontrada.', 'warning');
      this.isLoading = false;
    }
  }

  async onApostaSelected(apostaId: string): Promise<void> {
    console.log('[DEBUG] onApostaSelected chamado com apostaId:', apostaId);
    this.apostaAtual = this.apostasUsuarioRodada.find(a => a.id === apostaId) || this.getEmptyApostaRodadaDto();
    console.log('[DEBUG] onApostaSelected - Aposta Atual selecionada:', this.apostaAtual);
    
    if (this.apostaAtual.id !== '') {
      this.apostaRodadaId = this.apostaAtual.id;
      this.apostadorCampeonatoId = this.apostaAtual.apostadorCampeonatoId; 
      if (this.rodadaId && this.apostadorCampeonatoId) {
        await this.loadJogosDaApostaAtual(this.rodadaId, this.apostaAtual.id); 
      } else {
        this.showNotification('IDs de rodada ou apostador do campeonato não disponíveis para carregar jogos.', 'warning');
      }
    } else {
      this.onClickCriarNovaAposta();
    }
  }

  private buildForm(): void {
    console.log('[DEBUG] Construindo formulário com', this.jogosDaApostaAtual.length, 'jogos.');
    while (this.palpites.length !== 0) {
      this.palpites.removeAt(0);
    }

    this.jogosDaApostaAtual.forEach(jogo => {
      this.palpites.push(this.fb.group({
        id: [jogo.id || null],
        idJogo: [jogo.idJogo, Validators.required],
        apostaCasa: [jogo.placarApostaCasa, [Validators.required, Validators.min(0)]], 
        apostaVisitante: [jogo.placarApostaVisita, [Validators.required, Validators.min(0)]] 
      }));
    });
    console.log('[DEBUG] Formulário construído. Palpites length:', this.palpites.length);
    this.palpites.controls.forEach((control, index) => {
      console.log(`[DEBUG] Palpite ${index}: Casa=${control.get('apostaCasa')?.value}, Fora=${control.get('apostaVisitante')?.value}`);
    });
  }

  salvarApostas(): void {
    console.log('[DEBUG] Tentando salvar apostas. Formulário válido:', this.apostaForm.valid);
    if (this.apostaForm.invalid) {
      this.showNotification('Por favor, preencha todos os placares corretamente.', 'warning');
      this.apostaForm.markAllAsTouched();
      return;
    }

    this.isSaving = true;
    const request: SalvarApostaRequestDto = {
      rodadaId: this.rodadaId!,
      apostadorCampeonatoId: this.apostadorCampeonatoId!, 
      ehCampeonato: this.apostaAtual?.ehApostaCampeonato || false,
      ehApostaIsolada: this.apostaAtual?.ehApostaIsolada || false,
      campeonatoId: this.rodadaSelecionada?.campeonatoId!,
      identificadorAposta: this.apostaAtual?.identificadorAposta || 'Aposta Padrão',
      apostasJogos: this.palpites.controls.map(control => ({
        jogoId: control.get('idJogo')?.value,
        placarCasa: control.get('apostaCasa')?.value,
        placarVisitante: control.get('apostaVisitante')?.value
      }))
    };

    if (this.apostaAtual.id !== '') {
      (request as any).apostaRodadaId = this.apostaAtual.id;
    }

    console.log('[DEBUG] Request de salvar apostas:', request);

    this.apostaService.salvarApostas(request).subscribe({
      next: (response) => {
        console.log('[DEBUG] Resposta do backend ao salvar:', response);
        if (response.success) {
          this.showNotification('Apostas salvas com sucesso!', 'success');
          if (this.rodadaId && this.apostadorCampeonatoId) {
            const newApostaId = isPreservedCollection<ApostaRodadaDto>(response.data) 
                                ? (response.data.$values && response.data.$values.length > 0 ? response.data.$values[0]?.id : null)
                                : (response.data as ApostaRodadaDto)?.id;
            this.loadApostasUsuarioRodada(this.rodadaId, this.apostadorCampeonatoId, newApostaId || null); 
          } else {
            this.showNotification('Erro: IDs de rodada ou apostador do campeonato não disponíveis após salvar.', 'error');
          }
        } else {
          this.handleApiResponseNotifications(response);
        }
      },
      error: (err: any) => {
        console.error('DEBUG (salvarApostas): Erro ao salvar apostas:', err);
        const notifications = err.notifications || [];
        if (notifications.length > 0) {
          notifications.forEach((notif: NotificationDto) => {
            this.showNotification(notif.mensagem, notif.tipo === 'Erro' ? 'error' : (notif.tipo === 'Alerta' ? 'warning' : 'info'));
          });
        } else {
          this.showNotification(err.message || 'Erro desconhecido ao salvar apostas.', 'error');
        }
      }
    }).add(() => {
      this.isSaving = false;
      console.log('DEBUG (salvarApostas): Salvamento finalizado. isSaving:', this.isSaving);
    });
  }

  private handleApiResponseNotifications(response: ApiResponse<any>): void {
    console.log('DEBUG (handleApiResponseNotifications): Tratando notificações da API. Resposta:', response);
    let notificationsToDisplay: NotificationDto[] = [];

    if (response.errors && Array.isArray(response.errors)) {
      notificationsToDisplay = response.errors.map(msg => ({
        codigo: 'BackendError',
        tipo: 'Erro',
        mensagem: msg,
        nomeCampo: undefined
      }));
    }
    else if (response.notifications) {
      if (isPreservedCollection<NotificationDto>(response.notifications)) {
        notificationsToDisplay = response.notifications.$values;
      }
      else if (Array.isArray(response.notifications)) {
        notificationsToDisplay = response.notifications as NotificationDto[];
      }
    }

    if (notificationsToDisplay.length > 0) {
      this.notifications = notificationsToDisplay;
      this.notifications.forEach(notif => {
        let toastType: 'success' | 'error' | 'warning' | 'info' = 'info';
        if (notif.tipo === 'Erro') {
          toastType = 'error';
        } else if (notif.tipo === 'Alerta') {
          toastType = 'warning';
        } else if (notif.tipo === 'Sucesso') {
          toastType = 'success';
        }
        this.showNotification(notif.mensagem, toastType);
      });
      this.hasNotifications = true;
      console.log('DEBUG (handleApiResponseNotifications): Notificações exibidas:', this.notifications);
    } else if (response.message) {
      this.showNotification(response.message, response.success ? 'success' : 'error');
      this.notifications.push({ tipo: response.success ? 'Sucesso' : 'Erro', mensagem: response.message, codigo: null, nomeCampo: null });
      this.hasNotifications = true;
      console.log('DEBUG (handleApiResponseNotifications): Nenhuma notificação específica, exibindo mensagem geral.');
    } else {
      this.showNotification('Erro desconhecido da API.', 'error');
      this.notifications.push({ tipo: 'Erro', mensagem: 'Erro desconhecido da API.', codigo: null, nomeCampo: null });
      this.hasNotifications = true;
      console.log('DEBUG (handleApiResponseNotifications): Nenhuma notificação específica, exibindo erro genérico.');
    }
  }

  private showNotification(message: string, type: 'success' | 'error' | 'warning' | 'info'): void {
    let panelClass: string[] = [];
    let duration: number | undefined = 5000;
    let action: string | undefined = 'Fechar';

    if (type === 'success') {
      panelClass = ['snackbar-success'];
    } else if (type === 'error') {
      panelClass = ['snackbar-error'];
      duration = undefined;
    } else if (type === 'warning') {
      panelClass = ['snackbar-warning'];
    } else if (type === 'info') {
      panelClass = ['snackbar-info'];
    }

    if (this.snackBar) {
      this.snackBar.open(message, action, {
        duration: duration,
        panelClass: panelClass,
        horizontalPosition: 'center',
        verticalPosition: 'top',
      });
    } else {
      console.warn(`[showNotification] MatSnackBar não está disponível. Mensagem: ${message}, Tipo: ${type}`);
      if (type === 'error') {
        console.error(`ERRO: ${message}`);
      } else if (type === 'warning') {
        console.warn(`ALERTA: ${message}`);
      } else {
        console.log(`INFO: ${message}`);
      }
    }
  }

  goBackToDashboard(): void {
    this.router.navigate(['/dashboard']);
  }
}
