using dimax_front.Application.Interfaces;
using dimax_front.Domain.DTOs;
using dimax_front.Shared.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace dimax_front.Presentation.Controllers
{
    [Route("api/v1/installation")]
    [ApiController]
    public class InstallationController : ControllerBase
    {
        private readonly IInstallationRecords _installationRecords;
        public InstallationController(IInstallationRecords installationRecords)
        {
            _installationRecords = installationRecords;
        }
        [Authorize]
        [HttpGet()]
        public async Task<ActionResult> HandleGetForInvoiceNumber([FromQuery] string invoiceNumber)
        {
            var response = await _installationRecords.GetForInvoiceNumber(invoiceNumber);

            if (!response.IsSuccess)
                return StatusCode(response.StatusCode, new { response.IsSuccess, response.Message });

            return StatusCode(response.StatusCode, new { response.IsSuccess, response.Message, response.Data });
        }

        [HttpGet("technical")]
        public async Task<ActionResult> HandleGetForTechnicalFileNumber([FromQuery] string technicalFileNumber)
        {
            var response = await _installationRecords.GetForTechnicalFileNumber(technicalFileNumber);

            if (!response.IsSuccess)
                return StatusCode(response.StatusCode, new { response.IsSuccess, response.Message });

            return StatusCode(response.StatusCode, new { response.IsSuccess, response.Message, response.Data });
        }

        [Authorize]
        [HttpGet("showall")]
        public async Task<ActionResult> HandleGetInstallationPageResponseAsync
            (
                [FromQuery] string plate,
                int? pageNumber,
                int? pageSize,
                string? sortBy,
                string? sortDir
            )
        {
            int finalPageNumber = pageNumber ?? int.Parse(AppConstants.DEFAULT_PAGE_NUMBER);
            int finalPageSize = pageSize ?? int.Parse(AppConstants.DEFAULT_PAGE_SIZE);
            string finalSortBy = sortBy ?? AppConstants.DEFAULT_SORT_BY;
            string finalSortDir = sortDir ?? AppConstants.DEFAULT_SORT_DIR;

            var response = await _installationRecords.GetInstallationPageResponseAsync
                (
                    plate, finalPageNumber, finalPageSize, finalSortBy, finalSortDir
                );

            if (!response.IsSuccess)
                return StatusCode(response.StatusCode, new { response.IsSuccess, response.Message });

            return StatusCode(
                response.StatusCode,
                new
                {
                    response.IsSuccess,
                    response.Message,
                    response.PageNumber,
                    response.PageSize,
                    response.TotalRecords,
                    response.TotalPages,
                    response.IsLastPage,
                    response.InstallationRecords
                });
        }

        [Authorize]
        [HttpPost("register")]
        public async Task<IActionResult> HandleSaveInstallation([FromQuery] string plate, [FromForm] InstallationHistoryDTO historyDTO)
        {
            // Extraer información del Request
            var scheme = Request.Scheme;
            var host = Request.Host.Value;

            var response = await _installationRecords.SaveInstallation(plate, historyDTO, scheme, host);

            if (!response.IsSuccess)
                return StatusCode(response.StatusCode, new { response.IsSuccess, response.Message });

            return StatusCode(response.StatusCode, new { response.IsSuccess, response.Message });
        }

        [Authorize]
        [HttpPut("update")]
        public async Task<IActionResult> HandleUpdateInstallationByTechnicalFile([FromForm] InstallationHistoryDTO historyDTO)
        {
            var scheme = Request.Scheme;
            var host = Request.Host.Value;

            var response = await _installationRecords.UpdateInstallationByTechnicalFile(historyDTO, scheme, host);

            if (!response.IsSuccess)
                return StatusCode(response.StatusCode, new { response.IsSuccess, response.Message });

            return StatusCode(response.StatusCode, new { response.IsSuccess, response.Message });
        }


        [Authorize(Roles = "Admin")]
        [HttpDelete("delete-by-invoice")]
        public async Task<IActionResult> HandleDeleteForInvoiceNumber([FromQuery] string invoiceNumber)
        {
            var response = await _installationRecords.DeleteForInvoiceNumber(invoiceNumber);

            if (!response.IsSuccess)
                return StatusCode(response.StatusCode, new { response.IsSuccess, response.Message });

            return StatusCode(response.StatusCode, new { response.IsSuccess, response.Message });
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("delete-by-technical-file")]
        public async Task<IActionResult> HandleDeleteForTechnicalFileNumber([FromQuery] string technicalFileNumber)
        {
            var response = await _installationRecords.DeleteForTechnicalFileNumber(technicalFileNumber);

            if (!response.IsSuccess)
                return StatusCode(response.StatusCode, new { response.IsSuccess, response.Message });

            return StatusCode(response.StatusCode, new { response.IsSuccess, response.Message });
        }
    }
}
