using dimax_front.Domain.DTOs;
using System.Text.RegularExpressions;
namespace dimax_front.Application.Validators
{
    public class VehicleValidator
    {
        public static ServiceResponse.GeneralResponse? ValidatePlate(string plate)
        {
            if (string.IsNullOrEmpty(plate))
            {
                return new ServiceResponse.GeneralResponse
                (
                    IsSuccess: false,
                    StatusCode: StatusCodes.Status400BadRequest,
                    Message: "La placa del vehículo es obligatoria."
                );
            }

            // Expresión regular para validar: 3 letras (mayúsculas o minúsculas), un guion medio y 4 dígitos
            if (!Regex.IsMatch(plate, @"^[A-Za-z]{3}-\d{4}$"))
            {
                return new ServiceResponse.GeneralResponse
                (
                    IsSuccess: false,
                    StatusCode: StatusCodes.Status400BadRequest,
                    Message: "Formato de la placa [AAA-1234]."
                );
            }

            return null; // No hay errores
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

            if (!Regex.IsMatch(ownerName, @"^[a-zA-Z\s]+$"))
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

            if (!Regex.IsMatch(brand, @"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$"))
            {
                return new ServiceResponse.GeneralResponse
                (
                    IsSuccess: false,
                    StatusCode: StatusCodes.Status400BadRequest,
                    Message: "La marca solo puede contener letras, tildes (á, é, í, ó, ú) y la letra 'ñ'."
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
