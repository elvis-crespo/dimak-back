using dimax_front.Domain.DTOs;
using System.Text.RegularExpressions;
namespace dimax_front.Application.Validators
{
    public class VehicleValidator
    {
        public static ServiceResponse.GeneralResponse? ValidatePlate(string plate)
        {
            if (string.IsNullOrWhiteSpace(plate))
                return new ServiceResponse.GeneralResponse
                (
                    IsSuccess: false,
                    StatusCode: StatusCodes.Status400BadRequest,
                    Message: "La placa es obligatoria."
                );

            if (!Regex.IsMatch(plate, @"^[A-Z]{3}-\d{4}$|^[A-Z]{2}-\d{3}[A-Z]$"))
                return new ServiceResponse.GeneralResponse
                (
                    IsSuccess: false,
                    StatusCode: StatusCodes.Status400BadRequest,
                    Message: "El formato de la placa debe ser AAA-1234 o AA-123A."
                );

            if (plate.Length != 8 && plate.Length != 7)
                return new ServiceResponse.GeneralResponse
                (
                    IsSuccess: false,
                    StatusCode: StatusCodes.Status400BadRequest,
                    Message: "La placa debe tener 7 u 8 caracteres."
                );

            return null; // Sin errores
        }



        // Validación del nombre del propietario
        public static ServiceResponse.GeneralResponse? ValidateOwnerName(string ownerName)
        {
            if (string.IsNullOrEmpty(ownerName))
            {
                return new ServiceResponse.GeneralResponse
                (
                    IsSuccess: false,
                    StatusCode: StatusCodes.Status400BadRequest,
                    Message: "El nombre del propietario es obligatorio."
                );
            }

            if (!Regex.IsMatch(ownerName, @"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$"))
            {
                return new ServiceResponse.GeneralResponse
                (
                    IsSuccess: false,
                    StatusCode: StatusCodes.Status400BadRequest,
                    Message: "El nombre solo puede contener letras."
                );
            }

            return null; // No hay errores
        }

        // Validación de la marca
        public static ServiceResponse.GeneralResponse? ValidateBrand(string brand)
        {
            if (brand == null || brand.Trim() == "") return null; // Permitimos que sea null o cadena vacía

            if (!Regex.IsMatch(brand, @"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\säëïöüÄËÏÖÜ-]+$"))
            {
                return new ServiceResponse.GeneralResponse
                (
                    IsSuccess: false,
                    StatusCode: StatusCodes.Status400BadRequest,
                    Message: "La marca solo puede contener letras, tildes, la letra 'ñ', diéresis, guiones y espacios."
                );
            }

            return null; // No hay errores
        }

        // Validación del modelo
        public static ServiceResponse.GeneralResponse? ValidateModel(string model)
        {
            if (model == null || model.Trim() == "") return null; // Permitimos que sea null o cadena vacía

            if (!Regex.IsMatch(model, @"^[a-zA-ZáéíóúÁÉÍÓÚñÑ0-9\s]*$"))
            {
                return new ServiceResponse.GeneralResponse
                (
                    IsSuccess: false,
                    StatusCode: StatusCodes.Status400BadRequest,
                    Message: "El modelo solo puede contener letras, tildes, la letra 'ñ', números y espacios."
                );
            }

            return null; // No hay errores
        }

        // Validación del año
        public static ServiceResponse.GeneralResponse? ValidateYear(int? year)
        {
            if (year == null || year == 0) return null; // Permitimos que sea null o 0

            if (year < 1900 || year > DateTime.Now.Year)
            {
                return new ServiceResponse.GeneralResponse
                (
                    IsSuccess: false,
                    StatusCode: StatusCodes.Status400BadRequest,
                    Message: "El año del vehículo es inválido."
                );
            }

            return null; // No hay errores
        }
    }
}
