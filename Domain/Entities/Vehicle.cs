using System.ComponentModel.DataAnnotations;

namespace dimax_front.Core.Entities
{
    public class Vehicle
    {
        [Key]
        [Required]
        [StringLength(8, ErrorMessage = "La placa debe tener 8 caracteres.")]
        [RegularExpression(@"^[A-Z]{3}-\d{4}$", ErrorMessage = "El formato de la placa debe ser XXX-YYYY (tres letras seguidas de un guion y cuatro dígitos).")]
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
