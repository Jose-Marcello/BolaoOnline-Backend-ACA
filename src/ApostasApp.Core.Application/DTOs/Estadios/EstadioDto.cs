using System;

namespace ApostasApp.Core.Application.DTOs.Estadios
{
    public class EstadioDto
    {
        public Guid Id { get; set; }
        public string Nome { get; set; }
        public string Cidade { get; set; }
        public string Estado { get; set; }
    }
}
