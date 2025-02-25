using dimax_front.Domain.DTOs;

namespace dimax_front.Application.Service
{
    public interface IFileService
    {
        Task<ServiceResponse.GeneralResponse> SaveFileAsync(IFormFile imageFile, string scheme, string host);
        void DeleteFile(string file);
    }
    public class FileService : IFileService
    {

        public async Task<ServiceResponse.GeneralResponse> SaveFileAsync(IFormFile imageFile, string scheme, string host)
        {
            try
            {
                //var file = imageFile.File;

                var file = imageFile;

                // Verificar si se ha proporcionado un archivo
                if (file != null && file.Length > 0)
                {
                    // Validar que el archivo es una imagen
                    if (!file.ContentType.StartsWith("image"))
                        return new ServiceResponse.GeneralResponse
                            (
                                IsSuccess: false,
                                StatusCode: StatusCodes.Status400BadRequest,
                                Message: "Solo se permiten imágenes."
                            );

                    // Validar tamaño máximo del archivo (5 MB)
                    if (file.Length > 1 * 1024 * 1024)
                        return new ServiceResponse.GeneralResponse
                            (
                                IsSuccess: false,
                                StatusCode: StatusCodes.Status400BadRequest,
                                Message: "El archivo excede el tamaño máximo permitido (1 MB)."
                            );

                    // Generar un nombre único para el archivo
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);

                    // Ruta de almacenamiento en el servidor
                    var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads", fileName);

                    // Crear la carpeta si no existe
                    var directoryPath = Path.GetDirectoryName(uploadPath);
                    if (directoryPath != null)
                    {
                        Directory.CreateDirectory(directoryPath);
                    }

                    // Guardar el archivo
                    using (var stream = new FileStream(uploadPath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    // Generar la URL pública del archivo
                    var photoUrl = $"{scheme}://{host}/uploads/{fileName}";

                    return new ServiceResponse.GeneralResponse
                        (
                            IsSuccess: true,
                            StatusCode: StatusCodes.Status200OK,
                            Message: photoUrl
                        );
                }
                else
                {
                    // Si no se proporciona un archivo
                    return new ServiceResponse.GeneralResponse
                        (
                            IsSuccess: false,
                            StatusCode: StatusCodes.Status400BadRequest,
                            Message: "No se ha cargado ninguna imagen."
                        );
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse.GeneralResponse
                    (
                        IsSuccess: false,
                        StatusCode: StatusCodes.Status500InternalServerError,
                        Message: $"Error al cargar la imagen. {ex.Message}"
                    );
            }
        }

        public void DeleteFile(string file)
        {
            if (string.IsNullOrEmpty(file))
                return;

            //var uploadsPath = Environment.GetEnvironmentVariable("UPLOADS_PATH");

            //if (string.IsNullOrEmpty(uploadsPath))
            //    throw new InvalidOperationException("La variable de entorno 'UPLOADS_PATH' no está definida.");

            var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");


            var fileName = Path.GetFileName(file); // Obtienes solo el nombre del archivo desde la URL
            var path = Path.Combine(uploadsPath, fileName); // Ruta en el sistema de archivos

            //var path = Path.Combine(Directory.GetCurrentDirectory(), "Uploads", fileName); // Ruta en el sistema de archivos

            if (!File.Exists(path))
            {
                Console.WriteLine("El archivo no existe, pero el flujo continuará.");
                //throw new FileNotFoundException("Invalid file path");
            }

            File.Delete(path);
        }
    }
}

