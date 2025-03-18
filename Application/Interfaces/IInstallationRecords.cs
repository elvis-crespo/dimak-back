using dimax_front.Domain.DTOs;
using Microsoft.Extensions.Hosting;

namespace dimax_front.Application.Interfaces
{
    public interface IInstallationRecords
    {
        Task<ServiceResponse.GeneralResponse> SaveInstallation(string plate, InstallationHistoryDTO historyDTO, string scheme, string host);
        Task<ServiceResponse.GeneralResponse> UpdateInstallationByTechnicalFile(InstallationHistoryDTO historyDTO, string scheme, string host);
        Task<ServiceResponse.ApiResponse> GetForInvoiceNumber(string invoiceNumber);
        Task<ServiceResponse.ApiResponse> GetForTechnicalFileNumber(string technicalFileNumber);
        Task<ServiceResponse.InstallationPageResponse> GetInstallationPageResponseAsync
            (
                string plate, int pageNumber, int pageSize, string shortBy, string sortDir
            );
        Task<ServiceResponse.GeneralResponse> DeleteInstallation(string invoiceNumber);
    }
}
