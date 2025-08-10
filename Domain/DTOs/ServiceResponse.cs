using dimax_front.Core.Entities;

namespace dimax_front.Domain.DTOs
{
    public class ServiceResponse
    {
        public record class GeneralResponse(bool IsSuccess, int StatusCode, string Message);
        public record class TokenResponse(bool IsSuccess, int StatusCode, string Message, string AccessToken, string RefreshToken);

        public record class ApiResponse(bool IsSuccess, int StatusCode, string Message, object? Data);
        public record InstallationPageResponse(bool IsSuccess, int StatusCode, string Message, List<InstallationHistory> InstallationRecords, int PageNumber, int PageSize, long TotalRecords, int TotalPages, bool IsLastPage);
    }
}
