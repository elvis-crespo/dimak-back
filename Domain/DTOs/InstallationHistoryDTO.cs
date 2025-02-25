using System.ComponentModel.DataAnnotations;

namespace dimax_front.Domain.DTOs
{
    public class InstallationHistoryDTO
    {
        public string? InvoiceNumber { get; set; } = null;
        public string? TechnicalFileNumber { get; set; } = null;
        public string? TechnicianName { get; set; } = null ;
        public string? InstallationCompleted { get; set; } = null;
        public required DateOnly Date { get; set; }
        public IFormFile? PhotoUrl { get; set; }
    }
}
