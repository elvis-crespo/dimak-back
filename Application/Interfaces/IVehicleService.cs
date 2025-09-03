using dimax_front.Core.Entities;
using dimax_front.Domain.DTOs;

namespace dimax_front.Application.Interfaces
{
    public interface IVehicleService
    {
        Task<ServiceResponse.GeneralResponse> SaveVehicleWithInstallation(MixDTO mixDTO, string scheme, string host);
        Task<ServiceResponse.ApiResponse> GetAll(int pageNumber, int pageSize, string sortBy, string sortDir);
        Task<ServiceResponse.ApiResponse> GetForPlate(string plate);
        Task<ServiceResponse.GeneralResponse> UpdateVehicle(string plateVehicle, Vehicle vehicle);
        Task<ServiceResponse.GeneralResponse> DeleteVehicle(string plateVehicle);
    }
}
