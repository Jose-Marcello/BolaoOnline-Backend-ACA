    // src/app/models/auth/register-request.model.ts

    /**
     * Interface que representa os dados enviados para o endpoint de registro do backend.
     */
    export interface RegisterRequestDto {
      apelido: string;
      email: string;
      password: string;
      confirmPassword: string;
      cpf: string;
      celular: string;
      // <<-- ADICIONADAS AS NOVAS PROPRIEDADES -->>
      scheme: string;
      host: string;
      sendConfirmationEmail: boolean;
      // Adicione outras propriedades que seu backend espera para o registro
    }
    