using dimax_front.Domain.DTOs;

namespace dimax_front.Application.Validators
{
    public class FileValidator
    {
        public static ServiceResponse.GeneralResponse? ValidateFile(object file)
        {
            if (file == null) return null;

            if (file is Stream fileStream && fileStream.Length > 1 * 1024 * 1024) // 1MB
            {
                return new ServiceResponse.GeneralResponse
                (
                    IsSuccess: false,
                    StatusCode: StatusCodes.Status400BadRequest,
                    Message: "La imagen debe pesar menos de 1MB."
                );
            }

            return null;
        }
    }
}
