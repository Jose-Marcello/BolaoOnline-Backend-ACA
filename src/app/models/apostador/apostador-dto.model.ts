// Localização: src/app/models/apostador-dto.model.ts
import { SaldoDto } from '@models/apostador/saldo.model'; // Assumindo que você tem um SaldoDto
import { ApostadorCampeonatoDto } from '@models/apostador-campeonato/apostador-campeonato-dto.model'; // Importa ApostadorCampeonatoDto
import { PreservedCollection } from '@models/common/api-response.model';

export interface ApostadorDto {
  id: string;
  usuarioId: string;
  apelido: string;
  email: string;
  saldo: SaldoDto;
  campeonatosAderidos?: PreservedCollection<ApostadorCampeonatoDto> | ApostadorCampeonatoDto[];
}
