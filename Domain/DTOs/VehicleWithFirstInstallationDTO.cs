namespace dimax_front.Domain.DTOs
{
    public class VehicleWithFirstInstallationDto
    {
        public string Plate { get; set; }
        public string OwnerName { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public int? Year { get; set; }
        public DateOnly? Date { get; set; }
    }

}
