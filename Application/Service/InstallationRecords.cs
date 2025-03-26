using AutoMapper;
using dimax_front.Application.Interfaces;
using dimax_front.Application.Validators;
using dimax_front.Core.Entities;
using dimax_front.Domain.DTOs;
using dimax_front.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace dimax_front.Application.Service
{
    public class InstallationRecords : IInstallationRecords
    {
        private readonly WorkshopDbContext _workshopDb;
        private readonly IFileService _fileService;
        private readonly IMapper _mapper;

        public InstallationRecords(WorkshopDbContext workshopDb, IMapper mapper, IFileService fileService)
        {
            _workshopDb = workshopDb;
            _mapper = mapper;
            _fileService = fileService;
        }
        public async Task<ServiceResponse.GeneralResponse> SaveInstallation(
            string plate,
            InstallationHistoryDTO historyDTO,
            string scheme,
            string host)
        {
            try
            {
                //validations
                var validationResponses = new List<ServiceResponse.GeneralResponse?>
                {
                    InstallationValidator.ValidateInvoiceNumber(historyDTO.InvoiceNumber),
                    InstallationValidator.ValidateTechnicalFileNumber(historyDTO.TechnicalFileNumber),
                    InstallationValidator.ValidateTechnicianName(historyDTO.TechnicianName),
                    InstallationValidator.ValidateDate(historyDTO.Date),
                    InstallationValidator.ValidateInstallationCompleted(historyDTO.InstallationCompleted),
                };

                // Check if there is any error
                var errorResponse = validationResponses.FirstOrDefault(r => r != null);
                if (errorResponse != null)
                    return errorResponse; //return the first error found

                //check if the vehicle exists
                var vehicleExists = await _workshopDb.Vehicules.AnyAsync(v => v.Plate == plate);
                if (!vehicleExists)
                    return new ServiceResponse.GeneralResponse
                    (
                        IsSuccess: false,
                        StatusCode: StatusCodes.Status404NotFound,
                        Message: $"No se encontró un vehículo con la placa {plate}"
                    );
                
                var existingInvoice = await _workshopDb.InstallationHistories
                    .FirstOrDefaultAsync(i => i.InvoiceNumber == historyDTO.InvoiceNumber && historyDTO.InvoiceNumber != null);

                //check if the invoice number already exists
                if (historyDTO.InvoiceNumber != null && existingInvoice != null)
                {
                    return new ServiceResponse.GeneralResponse
                    (
                        IsSuccess: false,
                        StatusCode: StatusCodes.Status400BadRequest,
                        Message: "El número de factura ya existe."
                    );
                }
                    
                var existingTechnicalFile = await _workshopDb.InstallationHistories
                        .FirstOrDefaultAsync(i => i.TechnicalFileNumber == historyDTO.TechnicalFileNumber && historyDTO.TechnicalFileNumber != null);

                //check if the technical file number already exists
                if(historyDTO.TechnicalFileNumber != null && existingTechnicalFile != null)
                {
                    return new ServiceResponse.GeneralResponse
                    (
                        IsSuccess: false,
                        StatusCode: StatusCodes.Status400BadRequest,
                        Message: "El número de ficha técnica ya existe."
                    );
                }

                var historyImg = historyDTO.PhotoUrl;
                var newRecord = _mapper.Map<InstallationHistory>(historyDTO);
                newRecord.PlateId = plate;

                if (historyImg != null)
                {
                    var uploadImagen = await _fileService.SaveFileAsync(historyImg, scheme, host, newRecord.TechnicalFileNumber);

                    if (!uploadImagen.IsSuccess)
                    {
                        return new ServiceResponse.GeneralResponse
                        (
                            IsSuccess: uploadImagen.IsSuccess,
                            StatusCode: uploadImagen.StatusCode,
                            Message: uploadImagen.Message
                        );
                    }

                    newRecord.PhotoUrl = uploadImagen.Message;
                }

                await _workshopDb.InstallationHistories.AddAsync(newRecord);
                await _workshopDb.SaveChangesAsync();

                return new ServiceResponse.GeneralResponse
                (
                    IsSuccess: true,
                    StatusCode: StatusCodes.Status201Created,
                    Message: "Instalación registrada correctamente"
                );
            }
            catch (Exception ex)
            {
                return new ServiceResponse.GeneralResponse
                (
                    IsSuccess: false,
                    StatusCode: StatusCodes.Status500InternalServerError,
                    Message: $"Error al registrar la instalación: {ex.Message}"
                );
            }
        }

        public async Task<ServiceResponse.GeneralResponse> UpdateInstallationByTechnicalFile(
      InstallationHistoryDTO historyDTO,
      string scheme,
      string host)
        {
            try
            {
                string technicalFileNumber = historyDTO.TechnicalFileNumber;

                // Validaciones
                var validationResponses = new List<ServiceResponse.GeneralResponse?>
        {
            InstallationValidator.ValidateTechnicalFileNumber(historyDTO.TechnicalFileNumber),
            InstallationValidator.ValidateTechnicianName(historyDTO.TechnicianName),
            InstallationValidator.ValidateDate(historyDTO.Date),
            InstallationValidator.ValidateInstallationCompleted(historyDTO.InstallationCompleted)
        };

                var errorResponse = validationResponses.FirstOrDefault(r => r != null);
                if (errorResponse != null)
                    return errorResponse;

                // Buscar instalación por Nº de Ficha Técnica
                var existingRecord = await _workshopDb.InstallationHistories
                    .FirstOrDefaultAsync(i => i.TechnicalFileNumber == technicalFileNumber);

                if (existingRecord == null)
                    return new ServiceResponse.GeneralResponse
                    (
                        IsSuccess: false,
                        StatusCode: StatusCodes.Status404NotFound,
                        Message: $"No se encontró una instalación con la Ficha Técnica {technicalFileNumber}"
                    );

                // Verificar si se subió un nuevo archivo de imagen
                if (historyDTO.PhotoUrl is IFormFile newPhotoFile)
                {
                    // Si la imagen actual no es nula, eliminarla antes de subir la nueva
                    if (!string.IsNullOrEmpty(existingRecord.PhotoUrl))
                    {
                        string oldFileName = Path.GetFileName(existingRecord.PhotoUrl);
                        _fileService.DeleteFile(oldFileName);
                    }

                    // Subir la nueva imagen
                    var uploadImage = await _fileService.SaveFileAsync(newPhotoFile, scheme, host, historyDTO.TechnicalFileNumber);
                    if (!uploadImage.IsSuccess)
                        return new ServiceResponse.GeneralResponse
                        (
                            IsSuccess: false,
                            StatusCode: uploadImage.StatusCode,
                            Message: uploadImage.Message
                        );

                    // Guardar la nueva URL de la imagen
                    existingRecord.PhotoUrl = uploadImage.Message;
                }

                // Asignar los valores manualmente, asegurándonos de no sobrescribir Ficha Técnica ni Factura
                existingRecord.TechnicianName = historyDTO.TechnicianName;
                existingRecord.Date = historyDTO.Date;
                existingRecord.InstallationCompleted = historyDTO.InstallationCompleted;

                // Verificar si hay cambios en la entidad con el ChangeTracker de EF Core
                bool hasChanges = _workshopDb.Entry(existingRecord).State == EntityState.Modified ||
                                  _workshopDb.Entry(existingRecord).Properties.Any(p => p.IsModified);

                // Si no hubo cambios, retornar sin actualizar
                if (!hasChanges)
                {
                    return new ServiceResponse.GeneralResponse
                    (
                        IsSuccess: true,
                        StatusCode: StatusCodes.Status200OK,
                        Message: "No se detectaron cambios en la instalación"
                    );
                }

                // Guardar cambios en la base de datos
                await _workshopDb.SaveChangesAsync();

                return new ServiceResponse.GeneralResponse
                (
                    IsSuccess: true,
                    StatusCode: StatusCodes.Status200OK,
                    Message: "Instalación actualizada correctamente"
                );
            }
            catch (Exception ex)
            {
                return new ServiceResponse.GeneralResponse
                (
                    IsSuccess: false,
                    StatusCode: StatusCodes.Status500InternalServerError,
                    Message: $"Error al actualizar la instalación. {ex.Message}"
                );
            }
        }

        //service to get installation record for TechnicalFileNumber
        public async Task<ServiceResponse.ApiResponse> GetForTechnicalFileNumber(string technicalFileNumber)
        {
            // Validar el número de factura
            var validateTechnicalFileNumber = InstallationValidator.ValidateTechnicalFileNumber(technicalFileNumber);
            if (validateTechnicalFileNumber != null)
            {
                return new ServiceResponse.ApiResponse
                (
                    IsSuccess: validateTechnicalFileNumber.IsSuccess,
                    StatusCode: validateTechnicalFileNumber.StatusCode,
                    Message: validateTechnicalFileNumber.Message,
                    Data: null
                );
            }

            try
            {
                // Verificar que el número de factura no esté vacío
                if (string.IsNullOrEmpty(technicalFileNumber))
                {
                    return new ServiceResponse.ApiResponse
                    (
                        IsSuccess: false,
                        StatusCode: StatusCodes.Status400BadRequest,
                        Message: "La ficha técnica no puede estar vacío.",
                        Data: null
                    );
                }

                // Buscar la instalación por número de factura
                var installation = await _workshopDb.InstallationHistories
                    .FirstOrDefaultAsync(ih => ih.TechnicalFileNumber == technicalFileNumber);

                if (installation == null)
                {
                    return new ServiceResponse.ApiResponse
                    (
                        IsSuccess: false,
                        StatusCode: StatusCodes.Status404NotFound,
                        Message: $"No se encontró la instalación con esta ficha técnica: {technicalFileNumber}",
                        Data: null
                    );
                }

                return new ServiceResponse.ApiResponse
                (
                    IsSuccess: true,
                    StatusCode: StatusCodes.Status200OK,
                    Message: "Datos obtenidos correctamente.",
                    Data: installation
                );
            }
            catch (Exception ex)
            {
                // Puedes loguear el error aquí para tener más detalles
                return new ServiceResponse.ApiResponse
                (
                    IsSuccess: false,
                    StatusCode: StatusCodes.Status500InternalServerError,
                    Message: $"Error al obtener la instalación: {ex.Message}",
                    Data: null
                );
            }
        }

        //service to get installation record for invoice number
        public async Task<ServiceResponse.ApiResponse> GetForInvoiceNumber(string invoiceNumber)
        {
            // Validar el número de factura
            var validateInvoiceNumber = InstallationValidator.ValidateInvoiceNumber(invoiceNumber);
            if (validateInvoiceNumber != null)
            {
                return new ServiceResponse.ApiResponse
                (
                    IsSuccess: validateInvoiceNumber.IsSuccess,
                    StatusCode: validateInvoiceNumber.StatusCode,
                    Message: validateInvoiceNumber.Message,
                    Data: null
                );
            }

            try
            {
                // Verificar que el número de factura no esté vacío
                if (string.IsNullOrEmpty(invoiceNumber))
                {
                    return new ServiceResponse.ApiResponse
                    (
                        IsSuccess: false,
                        StatusCode: StatusCodes.Status400BadRequest,
                        Message: "El número de factura no puede estar vacío.",
                        Data: null
                    );
                }

                // Buscar la instalación por número de factura
                var installation = await _workshopDb.InstallationHistories
                    .FirstOrDefaultAsync(ih => ih.InvoiceNumber == invoiceNumber);

                if (installation == null)
                {
                    return new ServiceResponse.ApiResponse
                    (
                        IsSuccess: false,
                        StatusCode: StatusCodes.Status404NotFound,
                        Message: $"No se encontró la instalación con el número de factura: {invoiceNumber}",
                        Data: null
                    );
                }

                return new ServiceResponse.ApiResponse
                (
                    IsSuccess: true,
                    StatusCode: StatusCodes.Status200OK,
                    Message: "Datos obtenidos correctamente.",
                    Data: installation
                );
            }
            catch (Exception ex)
            {
                // Puedes loguear el error aquí para tener más detalles
                return new ServiceResponse.ApiResponse
                (
                    IsSuccess: false,
                    StatusCode: StatusCodes.Status500InternalServerError,
                    Message: $"Error al obtener la instalación: {ex.Message}",
                    Data: null
                );
            }
        }

        //service to delete an installation
        public async Task<ServiceResponse.GeneralResponse> DeleteInstallation(string invoiceNumber)
        {
            //validate invoice number
            var validateInvoiceNumber = InstallationValidator.ValidateInvoiceNumber(invoiceNumber);

            if (validateInvoiceNumber != null)
                return validateInvoiceNumber;

            try
            {
                //find the installation
                var installation = await _workshopDb.InstallationHistories.FirstOrDefaultAsync(X => X.InvoiceNumber == invoiceNumber);
                if (installation == null)
                    return new ServiceResponse.GeneralResponse
                    (
                        IsSuccess: false,
                        StatusCode: StatusCodes.Status404NotFound,
                        Message: $"No se encontró la instalación con número de factura: {invoiceNumber}"
                    );

                //delete the image
                _fileService.DeleteFile(installation.PhotoUrl);

                _workshopDb.InstallationHistories.Remove(installation);
                await _workshopDb.SaveChangesAsync();
                return new ServiceResponse.GeneralResponse
                (
                    IsSuccess: true,
                    StatusCode: StatusCodes.Status200OK,
                    Message: "Instalación eliminada correctamente"
                );
            }
            catch (Exception ex)
            {
                return new ServiceResponse.GeneralResponse
                (
                    IsSuccess: false,
                    StatusCode: StatusCodes.Status500InternalServerError,
                    Message: $"Error al eliminar la instalación: {ex.Message}"
                );
            }
        }

        public async Task<ServiceResponse.InstallationPageResponse> GetInstallationPageResponseAsync(
    string plate, int pageNumber, int pageSize, string sortBy, string sortDir)
        {
            try
            {
                var normalizedPlate = plate.ToUpper().Trim();

                var validationResponse = VehicleValidator.ValidatePlate(normalizedPlate);
                if (validationResponse != null)
                {
                    return new ServiceResponse.InstallationPageResponse
                    (
                        IsSuccess: validationResponse.IsSuccess,
                        StatusCode: validationResponse.StatusCode,
                        Message: validationResponse.Message,
                        InstallationRecords: new List<InstallationHistory>(),
                        PageNumber: 0,
                        PageSize: 0,
                        TotalRecords: 0,
                        TotalPages: 0,
                        IsLastPage: false
                    );
                }


                // Verificar si el vehículo existe
                var vehicleExists = await _workshopDb.Vehicules.AnyAsync(v => v.Plate == plate);
                if (!vehicleExists)
                {
                    return CreateEmptyResponse("El vehículo no existe en la base de datos.");
                }

                // Normalizar valores de paginación
                pageNumber = Math.Max(1, pageNumber);
                pageSize = Math.Max(1, pageSize);

                // Filtrar instalaciones por placa
                var query = _workshopDb.InstallationHistories
                    .Where(ih => ih.PlateId == plate);

                // Orden dinámico
                query = sortDir.Equals("desc", StringComparison.OrdinalIgnoreCase)
                    ? query.OrderByDescending(e => EF.Property<object>(e, sortBy))
                    : query.OrderBy(e => EF.Property<object>(e, sortBy));

                // Contar registros totales
                var totalRecords = await query.CountAsync();
                if (totalRecords == 0)
                {
                    return CreateEmptyResponse("No existen instalaciones para este vehículo.");
                }

                // Paginación
                var data = await query
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                // Mapear resultados
                var installationHistory = _mapper.Map<List<InstallationHistory>>(data);

                return new ServiceResponse.InstallationPageResponse
                (
                    IsSuccess: true,
                    StatusCode: StatusCodes.Status200OK,
                    Message: "Datos obtenidos correctamente.",
                    InstallationRecords: installationHistory,
                    PageNumber: pageNumber,
                    PageSize: pageSize,
                    TotalRecords: totalRecords,
                    TotalPages: (int)Math.Ceiling((double)totalRecords / pageSize),
                    IsLastPage: pageNumber * pageSize >= totalRecords
                );
            }
            catch (Exception ex)
            {
                return CreateErrorResponse($"An error occurred while fetching the data: {ex.Message}");
            }
        }

        private ServiceResponse.InstallationPageResponse CreateEmptyResponse(string message)
        {
            return new ServiceResponse.InstallationPageResponse
            (
                IsSuccess: false,
                StatusCode: StatusCodes.Status404NotFound,
                Message: message,
                InstallationRecords: new List<InstallationHistory>(),
                PageNumber: 0,
                PageSize: 0,
                TotalRecords: 0,
                TotalPages: 0,
                IsLastPage: false
            );
        }

        private ServiceResponse.InstallationPageResponse CreateErrorResponse(string message)
        {
            return new ServiceResponse.InstallationPageResponse
            (
                IsSuccess: false,
                StatusCode: StatusCodes.Status500InternalServerError,
                Message: message,
                InstallationRecords: new List<InstallationHistory>(),
                PageNumber: 0,
                PageSize: 0,
                TotalRecords: 0,
                TotalPages: 0,
                IsLastPage: false
            );
        }

    }
}
