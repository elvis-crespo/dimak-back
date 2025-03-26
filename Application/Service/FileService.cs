using dimax_front.Domain.DTOs;

namespace dimax_front.Application.Service
{
    public interface IFileService
    {
        Task<ServiceResponse.GeneralResponse> SaveFileAsync(IFormFile imageFile, string scheme, string host, string TechnicalFileNumber);
        void DeleteFile(string file);
    }
    public class FileService : IFileService
    {
        public async Task<ServiceResponse.GeneralResponse> SaveFileAsync(IFormFile imageFile, string scheme, string host, string fichaTecnica)
        {
            try
            {
                if (imageFile == null || imageFile.Length == 0)
                    return new ServiceResponse.GeneralResponse(false, StatusCodes.Status400BadRequest, "No se ha cargado ninguna imagen.");

                if (!imageFile.ContentType.StartsWith("image"))
                    return new ServiceResponse.GeneralResponse(false, StatusCodes.Status400BadRequest, "Solo se permiten imágenes.");

                if (imageFile.Length > 1 * 1024 * 1024) // 5MB
                    return new ServiceResponse.GeneralResponse(false, StatusCodes.Status400BadRequest, "El archivo excede el tamaño máximo permitido (1 MB).");

                // **Asegurar que la ficha técnica no sea nula o vacía**
                if (string.IsNullOrEmpty(fichaTecnica))
                    return new ServiceResponse.GeneralResponse(false, StatusCodes.Status400BadRequest, "El número de ficha técnica es obligatorio.");

                // Ruta donde se guardarán las imágenes
                var uploadDir = "C:\\ImagenesUploads";
                Directory.CreateDirectory(uploadDir); // Asegura que la carpeta exista

                // **Usar solo el número de ficha técnica como nombre del archivo**
                string fileName = $"{fichaTecnica}{Path.GetExtension(imageFile.FileName)}";
                string filePath = Path.Combine(uploadDir, fileName);

                // Si ya existe un archivo con el mismo nombre, genera un nuevo nombre (evitar sobreescritura)
                int counter = 1;
                while (File.Exists(filePath))
                {
                    fileName = $"{fichaTecnica}_{counter}{Path.GetExtension(imageFile.FileName)}";
                    filePath = Path.Combine(uploadDir, fileName);
                    counter++;
                }

                // Guardar el archivo
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }

                var photoUrl = $"{scheme}://{host}/ImagenesUploads/{fileName}";

                return new ServiceResponse.GeneralResponse(true, StatusCodes.Status200OK, photoUrl);
            }
            catch (Exception ex)
            {
                return new ServiceResponse.GeneralResponse(false, StatusCodes.Status500InternalServerError, $"Error al cargar la imagen. {ex.Message}");
            }
        }


        public void DeleteFile(string file)
        {
            if (string.IsNullOrEmpty(file))
                return;

            // Usar la misma ruta donde se guardan las imágenes
            var uploadDir = "C:\\ImagenesUploads";

            // Obtiene solo el nombre del archivo desde la URL (o nombre completo)
            var fileName = Path.GetFileName(file);

            // Ruta completa del archivo en el sistema
            var path = Path.Combine(uploadDir, fileName);

            // Verifica si el archivo existe
            if (!File.Exists(path))
            {
                Console.WriteLine("El archivo no existe, pero el flujo continuará.");
                return; // Si el archivo no existe, simplemente regresa sin hacer nada
            }

            // Elimina el archivo
            File.Delete(path);
            Console.WriteLine("Archivo eliminado con éxito.");
        }

    }
}