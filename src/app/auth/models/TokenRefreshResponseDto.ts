// Localização: src/app/models/auth/token-refresh-response-dto.model.ts

/**
 * DTO para a resposta de refresh de token da API.
 * Usado quando um novo token JWT é obtido via refresh.
 */
export interface TokenRefreshResponseDto {
  token: string;
  refreshToken: string;
  expiresIn: number; // Tempo de expiração do token em segundos (se o backend retornar assim)
  userId: string; // O ID único do usuário no seu backend
  // Se o seu endpoint de refresh retornar 'expiration' como string, ajuste aqui:
  // expiration?: string;
}
