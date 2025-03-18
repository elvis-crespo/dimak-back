namespace dimax_front.Domain.DTOs
{
    public class MixDTO
    {
        public required string Plate { get; set; }
        public required string OwnerName { get; set; }
        public string? Brand { get; set; } = string.Empty;
        public string? Model { get; set; } = string.Empty;
        public int? Year { get; set; } = 0;

        public required string TechnicalFileNumber { get; set; }
        public string? InvoiceNumber { get; set; } = string.Empty;
        public string? TechnicianName { get; set; } = string.Empty;
        public string? InstallationCompleted { get; set; } = string.Empty;
        public required DateOnly Date { get; set; }
        public IFormFile? PhotoUrl { get; set; }
    }
}
