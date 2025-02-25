using dimax_front.Domain.DTOs;
using System.Text.RegularExpressions;

namespace dimax_front.Application.Validators
{
    public class UserValidator
    {
            // Validación para Username
        public static ServiceResponse.GeneralResponse? ValidateUsername(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                return new ServiceResponse.GeneralResponse
                (
                    IsSuccess: false,
                    StatusCode: StatusCodes.Status400BadRequest,
                    Message: "El nombre de usuario es obligatorio."
                );
            }

            if (!Regex.IsMatch(username, @"^[a-zA-ZñÑáéíóúÁÉÍÓÚ0-9_-]*$"))
            {
                return new ServiceResponse.GeneralResponse
                (
                    IsSuccess: false,
                    StatusCode: StatusCodes.Status400BadRequest,
                    Message: "El nombre de usuario solo puede contener letras, números y caracteres especiales _ o -."
                );
            }

            return null;
        }

        // Validación para Password
        public static ServiceResponse.GeneralResponse? ValidatePassword(string password)
        {
            if (string.IsNullOrEmpty(password))
            {
                return new ServiceResponse.GeneralResponse
                (
                    IsSuccess: false,
                    StatusCode: StatusCodes.Status400BadRequest,
                    Message: "La contraseña es obligatoria."
                );
            }

            // Validación de la contraseña (mínimo 6 caracteres, sin espacios, al menos un número o un carácter especial)
            var passwordRegex = new Regex(@"^(?!.*\s)(?=.*\d)(?=.*[!@#$%^&*()_+\-=\[\]{};':""\\|,.<>\/?]).{6,}$");
            if (!passwordRegex.IsMatch(password))
            {
                return new ServiceResponse.GeneralResponse
                (
                    IsSuccess: false,
                    StatusCode: StatusCodes.Status400BadRequest,
                    Message: "La contraseña debe tener al menos 6 caracteres, no debe contener espacios, y debe tener al menos un número y un carácter especial."
                );
            }

            return null;
        }


        // Validación para Email
        public static ServiceResponse.GeneralResponse? ValidateEmail(string email)
         {
             if (string.IsNullOrEmpty(email))
             {
                 return new ServiceResponse.GeneralResponse
                 (
                     IsSuccess: false,
                     StatusCode: StatusCodes.Status400BadRequest,
                     Message: "El correo electrónico es obligatorio."
                 );
             }

             // Validación del Email (formato básico de correo electrónico)
             var emailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
             if (!emailRegex.IsMatch(email))
             {
                 return new ServiceResponse.GeneralResponse
                 (
                     IsSuccess: false,
                     StatusCode: StatusCodes.Status400BadRequest,
                     Message: "El formato del correo electrónico no es válido."
                 );
             }

             return null;
         }

            // Validación para Role
         public static ServiceResponse.GeneralResponse? ValidateRole(string role)
         {
             if (string.IsNullOrEmpty(role))
             {
                 return new ServiceResponse.GeneralResponse
                 (
                     IsSuccess: false,
                     StatusCode: StatusCodes.Status400BadRequest,
                     Message: "El rol es obligatorio."
                 );
             }

             // Validación del Role (asegurarse de que esté en una lista de roles válidos)
             var validRoles = new List<string> { "Admin", "User", "Manager" }; // Puedes agregar más roles si lo necesitas
             if (!validRoles.Contains(role))
             {
                 return new ServiceResponse.GeneralResponse
                 (
                     IsSuccess: false,
                     StatusCode: StatusCodes.Status400BadRequest,
                     Message: "Rol inválido. Los roles permitidos son Admin, User, Manager."
                 );
             }

             return null;
         }
    }
}
