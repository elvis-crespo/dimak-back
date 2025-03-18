using dimax_front.Application.Interfaces;
using dimax_front.Core.Entities;
using dimax_front.Domain.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace dimax_front.Presentation.Controllers
{
    [Route("api/v1/vehicle")]
    [ApiController]
    public class VehicleController : ControllerBase
    {
        private readonly IVehicleService _vehicleService;

        public VehicleController(IVehicleService vehicleService)
        {
            _vehicleService = vehicleService;
        }

        [Authorize]
        [HttpGet("vehicles")]
        public async Task<ActionResult> HandleGetAll()
        {
            var response = await _vehicleService.GetAll();

            if (!response.IsSuccess)
                return StatusCode(response.StatusCode, new { response.IsSuccess, response.Message });

            return StatusCode(response.StatusCode, new { response.IsSuccess, response.Message, response.Data });
        }

        [Authorize]
        [HttpGet("search-plate/")]
        public async Task<ActionResult> HandleGetForPlate([FromQuery] string plate)
        {
            var response = await _vehicleService.GetForPlate(plate);

            if (!response.IsSuccess)
                return StatusCode(response.StatusCode, new { response.Message });

            return StatusCode(response.StatusCode, new { response.IsSuccess, response.Message, response.Data });
        }

        //[Authorize]
        [HttpPost("register")]
        public async Task<ActionResult> HandleSaveVehicleWithInstallRecord([FromForm] MixDTO mixDTO)
        {
            var scheme = Request.Scheme;
            var host = Request.Host.Value;
            var response = await _vehicleService.SaveVehicleWithInstallation(mixDTO, scheme, host);

            if (!response.IsSuccess)
                return StatusCode(response.StatusCode, new { response.IsSuccess, response.Message });

            return StatusCode(response.StatusCode, new { response.IsSuccess, response.Message });
        }

        [Authorize]
        [HttpPut("update")]
        public async Task<ActionResult> HandleUpdateVehicle([FromQuery] string plate, [FromBody] Vehicle vehicle)
        {
            var response = await _vehicleService.UpdateVehicle(plate, vehicle);

            if (!response.IsSuccess)
                return StatusCode(response.StatusCode, new { response.IsSuccess, response.Message });

            return StatusCode(response.StatusCode, new { response.IsSuccess, response.Message });
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("delete")]
        public async Task<ActionResult> HandleDeleteVehicle(string plate)
        {
            var response = await _vehicleService.DeleteVehicle(plate);

            if (!response.IsSuccess)
                return StatusCode(response.StatusCode, new { response.IsSuccess, response.Message });

            return StatusCode(response.StatusCode, new { response.IsSuccess, response.Message });
        }
    }
}
