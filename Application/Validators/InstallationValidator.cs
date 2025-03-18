using dimax_front.Domain.DTOs;
using System.Text.RegularExpressions;

namespace dimax_front.Application.Validators
{
    public class InstallationValidator
    {
        public static ServiceResponse.GeneralResponse? ValidateInvoiceNumber(string invoiceNumber)
        {

            if (string.IsNullOrWhiteSpace(invoiceNumber)) return null;

            if (!Regex.IsMatch(invoiceNumber, @"^\d{3}-\d{3}-(\d{9}|[A-Z]{2}\d{7})$"))
                return new ServiceResponse.GeneralResponse
                (
                    IsSuccess: false,
                    StatusCode: StatusCodes.Status400BadRequest,
                    Message: "El formato del número de factura debe ser 001-002-123456789 o 001-002-OP3456789."
                );

            if (invoiceNumber.Length != 17)
                return new ServiceResponse.GeneralResponse
                (
                    IsSuccess: false,
                    StatusCode: StatusCodes.Status400BadRequest,
                    Message: "El número de factura debe tener exactamente 17 caracteres."
                );

            return null;
        }


        public static ServiceResponse.GeneralResponse? ValidateTechnicalFileNumber(string technicalFileNumber)
        {
            // Validar que el número de ficha técnica no sea nulo ni vacío
            if (string.IsNullOrWhiteSpace(technicalFileNumber))
            {
                return new ServiceResponse.GeneralResponse
                (
                    IsSuccess: false,
                    StatusCode: StatusCodes.Status400BadRequest,
                    Message: "El número de ficha técnica es obligatorio."
                );
            }

            // Validar que solo contenga números y no exceda los 15 dígitos
            if (!Regex.IsMatch(technicalFileNumber, @"^\d{1,15}$"))
            {
                return new ServiceResponse.GeneralResponse
                (
                    IsSuccess: false,
                    StatusCode: StatusCodes.Status400BadRequest,
                    Message: "El número de ficha técnica debe contener solo números y no exceder los 15 dígitos."
                );
            }

            return null;
        }


        public static ServiceResponse.GeneralResponse? ValidateTechnicianName(string technicianName)
        {
            if (technicianName == null || technicianName.Trim() == "") return null;

            if (!Regex.IsMatch(technicianName, @"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$"))
            {
                return new ServiceResponse.GeneralResponse
                (
                    IsSuccess: false,
                    StatusCode: StatusCodes.Status400BadRequest,
                    Message: "El nombre solo puede contener letras, tildes (á, é, í, ó, ú) y la letra 'ñ'."
                );
            }

            return null;
        }

        public static ServiceResponse.GeneralResponse? ValidateDate(DateOnly? date)
        {
            if (date is null)
                return new ServiceResponse.GeneralResponse
                (
                    IsSuccess: false,
                    StatusCode: StatusCodes.Status400BadRequest,
                    Message: "La fecha es obligatoria."
                );

            DateOnly currentDate = DateOnly.FromDateTime(DateTime.UtcNow.AddHours(-5)); // Ecuador está en UTC-5

            DateOnly minDate = new DateOnly(1900, 1, 1);
            DateOnly maxDate = currentDate.AddDays(30);

            if (date.Value < minDate)
                return new ServiceResponse.GeneralResponse
                (
                    IsSuccess: false,
                    StatusCode: StatusCodes.Status400BadRequest,
                    Message: "La fecha no puede ser anterior al 1 de enero de 1900."
                );

            if (date.Value > maxDate)
                return new ServiceResponse.GeneralResponse
                (
                    IsSuccess: false,
                    StatusCode: StatusCodes.Status400BadRequest,
                    Message: "La fecha no puede estar más de 30 días en el futuro."
                );

            return null;
        }


        public static ServiceResponse.GeneralResponse? ValidateInstallationCompleted(string installationCompleted, int maxLength = 1000, int maxWords = 100)
        {
            if (installationCompleted == null || installationCompleted.Trim() == "") return null;

            // Validar que no exceda el número máximo de caracteres
            if (installationCompleted.Length > maxLength)
            {
                return new ServiceResponse.GeneralResponse
                (
                    IsSuccess: false,
                    StatusCode: StatusCodes.Status400BadRequest,
                    Message: $"La descripción no puede exceder {maxLength} caracteres."
                );
            }

            // Validar que no exceda el número máximo de palabras
            var wordCount = installationCompleted.Split(new char[] { ' ', '\t', '\n' }, StringSplitOptions.RemoveEmptyEntries).Length;
            if (wordCount > maxWords)
            {
                return new ServiceResponse.GeneralResponse
                (
                    IsSuccess: false,
                    StatusCode: StatusCodes.Status400BadRequest,
                    Message: $"La descripción no puede exceder {maxWords} palabras."
                );
            }

            // Validar con la expresión regular
            if (!Regex.IsMatch(installationCompleted, @"^[a-zA-Z0-9\s¿?()¡!ñÑáéíóúÁÉÍÓÚ´$+,.\-:]*$"))
            {
                return new ServiceResponse.GeneralResponse
                (
                    IsSuccess: false,
                    StatusCode: StatusCodes.Status400BadRequest,
                    Message: "La descripción solo puede contener letras, números y los siguientes caracteres: ¿ ? ( ) ¡ ! ñ Ñ á é í ó ú Á É Í Ó Ú ´ $ + , . : -"
                );
            }

            return null;
        }

    }
}
