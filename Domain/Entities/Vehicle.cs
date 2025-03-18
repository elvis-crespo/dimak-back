using System.ComponentModel.DataAnnotations;

namespace dimax_front.Core.Entities
{
    public class Vehicle
    {
        [Key]
        [Required(ErrorMessage = "La placa es obligatoria.")]
        [StringLength(8, MinimumLength = 7, ErrorMessage = "La placa debe tener 7 u 8 caracteres.")]
        [RegularExpression(@"^[A-Z]{3}-\d{4}$|^[A-Z]{2}-\d{3}[A-Z]$", ErrorMessage = "El formato de la placa debe ser AAA-1234 o AA-123A.")]
        public required string Plate { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "El nombre del propietario no puede exceder los 100 caracteres.")]
        public required string OwnerName { get; set; }

        [StringLength(50, ErrorMessage = "La marca no puede exceder los 50 caracteres.")]
        public string? Brand { get; set; } = null;

        [StringLength(50, ErrorMessage = "El modelo no puede exceder los 50 caracteres.")]
        public string? Model { get; set; } = null;

        [Range(1900, int.MaxValue, ErrorMessage = "El año debe ser mayor o igual a 1900.")]
        public int? Year { get; set; } = null;

        public List<InstallationHistory> InstallationHistories { get; } = new List<InstallationHistory>();
    }
}
