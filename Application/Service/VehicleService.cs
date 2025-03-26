using System.Diagnostics;
using dimax_front.Application.Interfaces;
using dimax_front.Application.Validators;
using dimax_front.Core.Entities;
using dimax_front.Domain.DTOs;
using dimax_front.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace dimax_front.Application.Service
{
    public class VehicleService : IVehicleService
    {
        private readonly WorkshopDbContext _workshopDb;
        private readonly IFileService _fileService;

        public VehicleService(WorkshopDbContext workshopDb, IFileService fileService)
        {
            _workshopDb = workshopDb;
            _fileService = fileService;
        }

        public async Task<ServiceResponse.ApiResponse> GetAll()
        {
            try
            {
                var vehicles = await _workshopDb.Vehicules
                .ToListAsync();

                if (vehicles.Count == 0)
                    return new ServiceResponse.ApiResponse
                        (
                            IsSuccess: false,
                            StatusCode: StatusCodes.Status400BadRequest,
                            Message: "No se encontraron registros.",
                            Data: null
                        );

                return new ServiceResponse.ApiResponse
                        (
                            IsSuccess: true,
                            StatusCode: StatusCodes.Status200OK,
                            Message: "Registros mostrados correctamente.",
                            Data: vehicles
                        );

            }
            catch (Exception ex)
            {
                return new ServiceResponse.ApiResponse
                        (
                            IsSuccess: false,
                            StatusCode: StatusCodes.Status500InternalServerError,
                            Message: $"Ha ocurrido un error: {ex.Message}",
                            Data: null
                        );
            }

        }

        public async Task<ServiceResponse.ApiResponse> GetForPlate(string plate)
        {
            try
            {
                var normalizedPlate = plate.ToUpper().Trim();

                var validationResponse = VehicleValidator.ValidatePlate(normalizedPlate);
                if (validationResponse != null)
                {
                    return new ServiceResponse.ApiResponse
                    (
                        IsSuccess: validationResponse.IsSuccess,
                        StatusCode: validationResponse.StatusCode,
                        Message: validationResponse.Message,
                        Data: null
                    );
                }

                var vehicle = await _workshopDb.Vehicules.Where(x => x.Plate == plate).FirstOrDefaultAsync();

                if (vehicle == null)
                    return new ServiceResponse.ApiResponse
                        (
                            IsSuccess: false,
                            StatusCode: StatusCodes.Status404NotFound,
                            Message: $"No se encontraron vehículos registrados con la placa '{plate}'.",
                            Data: null
                        );

                return new ServiceResponse.ApiResponse
                        (
                            IsSuccess: true,
                            StatusCode: StatusCodes.Status200OK,
                            Message: "Registros mostrados correctamente.",
                            Data: vehicle
                        );

            }
            catch (Exception ex)
            {
                return new ServiceResponse.ApiResponse
                        (
                            IsSuccess: false,
                            StatusCode: StatusCodes.Status500InternalServerError,
                            Message: $"Ha ocurrido un error: {ex.Message}",
                            Data: null
                        );
            }

        }

        public async Task<ServiceResponse.GeneralResponse> UpdateVehicle(string plateVehicle, Vehicle vehicle)
        {
            try
            {
                //validations
                var validationResponses = new List<ServiceResponse.GeneralResponse?>
                {
                    VehicleValidator.ValidatePlate(vehicle.Plate),
                    VehicleValidator.ValidateOwnerName(vehicle.OwnerName),
                    VehicleValidator.ValidateBrand(vehicle.Brand),
                    VehicleValidator.ValidateModel(vehicle.Model),
                    VehicleValidator.ValidateYear(vehicle.Year),
                };

                // Check if there is any error
                var errorResponse = validationResponses.FirstOrDefault(r => r != null);
                if (errorResponse != null)
                {
                    return errorResponse; //return the first error found
                }

                var vehicleDB = await _workshopDb.Vehicules.FirstOrDefaultAsync(x => x.Plate == plateVehicle);

                if (vehicleDB is null)
                {
                    return new ServiceResponse.GeneralResponse
                        (
                            IsSuccess: false,
                            StatusCode: StatusCodes.Status404NotFound,
                            Message: $"No se encontraron vehículos registrados con la placa '{plateVehicle}'"
                        );
                }

                vehicleDB.OwnerName = vehicle.OwnerName;
                vehicleDB.Brand = vehicle.Brand;
                vehicleDB.Model = vehicle.Model;
                vehicleDB.Year = vehicle.Year;

                await _workshopDb.SaveChangesAsync();

                return new ServiceResponse.GeneralResponse
                    (
                        IsSuccess: true,
                        StatusCode: StatusCodes.Status200OK,
                        Message: "Vehículo actualizado correctamente"
                    );
            }
            catch (Exception ex)
            {
                return new ServiceResponse.GeneralResponse
                    (
                        IsSuccess: false,
                        StatusCode: StatusCodes.Status500InternalServerError,
                        Message: $"Error al actualizar el vehículo: {ex.Message}"
                    );
            }
        }

        public async Task<ServiceResponse.GeneralResponse> DeleteVehicle(string plateVehicle)
        {
            try
            {
                var normalizedPlate = plateVehicle.ToUpper().Trim();

                var validationResponse = VehicleValidator.ValidatePlate(normalizedPlate);
                if (validationResponse != null)
                {
                    return new ServiceResponse.GeneralResponse
                    (
                        IsSuccess: validationResponse.IsSuccess,
                        StatusCode: validationResponse.StatusCode,
                        Message: validationResponse.Message
                    );
                }


                // Cargar el vehículo y sus relaciones (historial de instalaciones)
                var vehicle = await _workshopDb.Vehicules
                    .Include(v => v.InstallationHistories)
                    .FirstOrDefaultAsync(v => v.Plate == plateVehicle);

                if (vehicle == null)
                {
                    return new ServiceResponse.GeneralResponse(
                        IsSuccess: false,
                        StatusCode: StatusCodes.Status404NotFound,
                        Message: "Vehículo no encontrado"
                    );
                }

                foreach (var imagePath in vehicle.InstallationHistories)
                {
                    _fileService.DeleteFile(imagePath.PhotoUrl);
                }


                // Eliminar el vehículo (y sus historiales si hay cascada configurada)
                _workshopDb.Vehicules.Remove(vehicle);
                await _workshopDb.SaveChangesAsync();

                return new ServiceResponse.GeneralResponse
                        (
                            IsSuccess: true,
                            StatusCode: StatusCodes.Status200OK,
                            Message: "Vehículo eliminado correctamente"
                        );
            }
            catch (Exception ex)
            {
                return new ServiceResponse.GeneralResponse
                (
                    IsSuccess: false,
                    StatusCode: StatusCodes.Status500InternalServerError,
                    Message: $"Ocurrió un error al intentar eliminar el vehículo: {ex.Message}"
                );

            }
        }

        public async Task<ServiceResponse.GeneralResponse> SaveVehicleWithInstallation(MixDTO mixDTO, string scheme, string host)
        {
            try
            {
                // Ejecutar validaciones de forma agrupada
                var validationResult = await ValidateVehicleAndInstallationAsync(mixDTO);
                if (!validationResult.IsSuccess)
                {
                    return validationResult;  // Retornar el primer error encontrado
                }

                mixDTO.Plate = mixDTO.Plate.ToUpper();

                // Guardar foto si es proporcionada
                string photoUrl = "";
                if (mixDTO.PhotoUrl != null)
                {
                    var uploadImagen = await _fileService.SaveFileAsync(mixDTO.PhotoUrl, scheme, host, mixDTO.TechnicalFileNumber);
                    if (!uploadImagen.IsSuccess)
                    {
                        return new ServiceResponse.GeneralResponse
                        (
                            IsSuccess: uploadImagen.IsSuccess,
                            StatusCode: uploadImagen.StatusCode,
                            Message: uploadImagen.Message
                        );
                    }
                    photoUrl = uploadImagen.Message;
                }

                // Guardar vehículo
                var vehicle = new Vehicle
                {
                    Plate = mixDTO.Plate,
                    OwnerName = mixDTO.OwnerName,
                    Brand = mixDTO.Brand,
                    Model = mixDTO.Model,
                    Year = mixDTO.Year,
                };

                // Guardar instalación
                var installRecord = new InstallationHistory
                {
                    PlateId = mixDTO.Plate,
                    InvoiceNumber = mixDTO.InvoiceNumber,
                    TechnicalFileNumber = mixDTO.TechnicalFileNumber,
                    TechnicianName = mixDTO.TechnicianName,
                    Date = mixDTO.Date,
                    InstallationCompleted = mixDTO.InstallationCompleted,
                    PhotoUrl = photoUrl
                };

                // Agregar a la base de datos
                await _workshopDb.Vehicules.AddAsync(vehicle);
                await _workshopDb.InstallationHistories.AddAsync(installRecord);
                await _workshopDb.SaveChangesAsync();

                return new ServiceResponse.GeneralResponse
                (
                    IsSuccess: true,
                    StatusCode: StatusCodes.Status201Created,
                    Message: "Vehículo guardado correctamente"
                );
            }
            catch (Exception ex)
            {
                return new ServiceResponse.GeneralResponse
                (
                    IsSuccess: false,
                    StatusCode: StatusCodes.Status500InternalServerError,
                    Message: $"Ocurrió un error al guardar el vehículo. Inténtalo nuevamente. Error: {ex.Message}"
                );
            }
        }

        private async Task<ServiceResponse.GeneralResponse> ValidateVehicleAndInstallationAsync(MixDTO mixDTO)
        {
            // Verificar si el número de factura ya existe
            var existingInvoice = await _workshopDb.InstallationHistories
                .FirstOrDefaultAsync(i => i.InvoiceNumber == mixDTO.InvoiceNumber && mixDTO.InvoiceNumber != null);

            if (mixDTO.InvoiceNumber != null && existingInvoice != null)
            {
                return new ServiceResponse.GeneralResponse
                (
                    IsSuccess: false,
                    StatusCode: StatusCodes.Status400BadRequest,
                    Message: "El número de factura ya existe."
                );
            }


            // Verificar si el vehículo ya está registrado
            var registeredVehicle = await _workshopDb.Vehicules.AnyAsync(x => x.Plate == mixDTO.Plate);
            if (registeredVehicle)
            {
                return new ServiceResponse.GeneralResponse
                (
                    IsSuccess: false,
                    StatusCode: StatusCodes.Status400BadRequest,
                    Message: "Vehículo registrado anteriormente"
                );
            }

            // Verificar si el número de ficha técnica ya existe
            var existingTechnicalFile = await _workshopDb.InstallationHistories
                .FirstOrDefaultAsync(i => i.TechnicalFileNumber == mixDTO.TechnicalFileNumber);
            if (existingTechnicalFile != null)
            {
                return new ServiceResponse.GeneralResponse
                (
                    IsSuccess: false,
                    StatusCode: StatusCodes.Status400BadRequest,
                    Message: "El número de ficha técnica ya existe."
                );
            }

            // Realizar las validaciones de los campos
            var validationResponses = new List<ServiceResponse.GeneralResponse?>
            {
                VehicleValidator.ValidatePlate(mixDTO.Plate),
                VehicleValidator.ValidateOwnerName(mixDTO.OwnerName),
                VehicleValidator.ValidateBrand(mixDTO.Brand),
                VehicleValidator.ValidateModel(mixDTO.Model),
                VehicleValidator.ValidateYear(mixDTO.Year),
                InstallationValidator.ValidateInvoiceNumber(mixDTO.InvoiceNumber),
                InstallationValidator.ValidateTechnicalFileNumber(mixDTO.TechnicalFileNumber),
                InstallationValidator.ValidateTechnicianName(mixDTO.TechnicianName),
                InstallationValidator.ValidateDate(mixDTO.Date),
                InstallationValidator.ValidateInstallationCompleted(mixDTO.InstallationCompleted),
            };

            // Verificar el primer error que se encuentre
            var errorResponse = validationResponses.FirstOrDefault(r => r != null);
            if (errorResponse != null)
            {
                return errorResponse;  // Retornar el primer error encontrado
            }

            return new ServiceResponse.GeneralResponse
            (
                IsSuccess: true,
                StatusCode: StatusCodes.Status200OK,
                Message: "Validaciones pasadas"
            );
        }

    }
}
