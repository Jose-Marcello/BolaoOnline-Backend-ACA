// src/app/auth/models/user-claims.model.ts
// CRIE ESTE ARQUIVO NESTA LOCALIZAÇÃO CORRETA

/**
 * Interface para representar os claims (informações) extraídas de um token JWT.
 */
export interface UserClaims {
  userId: string; // O ID do usuário, geralmente 'sub' ou um claim 'user_id' do seu backend
  email: string;  // O email do usuário
  exp: number;    // Timestamp de expiração do token (em segundos desde a Época - Unix time)
  // Adicione aqui outros claims que você decodifica do JWT e que precisa no frontend
  // Ex: role: string;
}
