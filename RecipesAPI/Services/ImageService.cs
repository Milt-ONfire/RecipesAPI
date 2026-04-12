namespace RecipesAPI.Services
{
    public class ImageService
    {
        public async Task<string?> GuardarImagen(IFormFile archivo, string carpeta)
        {
            if (archivo == null || archivo.Length == 0)
                return null;

            var rutaCarpeta = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "imagenes", carpeta);

            if (!Directory.Exists(rutaCarpeta))
                Directory.CreateDirectory(rutaCarpeta);

            var extension = Path.GetExtension(archivo.FileName).ToLower();
            var nombreArchivo = Guid.NewGuid().ToString() + extension;
            var extensionesPermitidas = new[] { ".jpeg", ".jpg", ".png" };
            var rutaCompleta = Path.Combine(rutaCarpeta, nombreArchivo);

            if (!extensionesPermitidas.Contains(extension))
            {
                return "Formato de imagen no permitido";
            }

            using (var stream = new FileStream(rutaCompleta, FileMode.Create))
            {
                await archivo.CopyToAsync(stream);
            }

            return $"/imagenes/{carpeta}/{nombreArchivo}";
        }
    }
}
