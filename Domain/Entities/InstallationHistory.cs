using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace dimax_front.Core.Entities
{
    public class InstallationHistory
    {
        [Key]
        public int HistoryId { get; set; }

        [StringLength(17)]
        public string? InvoiceNumber { get; set; } = null;


        [StringLength(15, ErrorMessage = "El número de ficha técnica debe tener al menos 15 caracteres.")]
        public string? TechnicalFileNumber { get; set; } = null;


        [MaxLength(100)]
        public string? TechnicianName { get; set; } = null;


        public string? InstallationCompleted { get; set; } = null;


        [Range(typeof(DateTime), "1/1/1900", "12/31/9999", ErrorMessage = "La fecha debe estar entre 01/01/1900 y 31/12/9999.")]
        public required DateOnly Date { get; set; }


        [Url(ErrorMessage = "La URL de la foto debe ser válida.")]
        [MaxLength(255)]
        public string? PhotoUrl { get; set; } = null;

        public string? PlateId { get; set; }
        public Vehicle Vehicle { get; set; } = null!;
    }
}
