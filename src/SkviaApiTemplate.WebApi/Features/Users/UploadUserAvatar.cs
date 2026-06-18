using SkviaApiTemplate.WebApi.Auth.Permissions;

namespace SkviaApiTemplate.WebApi.Features.Users;

public class UploadUserAvatar : IEndpoint
{
    public static void Map(RouteGroupBuilder group)
        => group.MapPost("/photo", Handle)
            .WithSummary("Subir foto de usuario")
            .WithDescription("Sube la foto de perfil y retorna la URL.")
            .DisableAntiforgery()
            .WithPermission(Permissions.Users.Edit);

    private static async Task<IResult> Handle(
        IFormFile photo,
        IWebHostEnvironment env,
        CancellationToken ct)
    {
        // Validations
        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
        var fileExtension = Path.GetExtension(photo.FileName).ToLowerInvariant();

        if (!allowedExtensions.Contains(fileExtension))
            return TypedResults.Problem(
                detail: "Solo se permiten imágenes JPG, PNG o WEBP.",
                statusCode: StatusCodes.Status400BadRequest,
                title: "Formato no permitido.");

        if (photo.Length > 2 * 1024 * 1024)
            return TypedResults.Problem(
                detail: "La imagen no puede superar 2MB.",
                statusCode: StatusCodes.Status400BadRequest,
                title: "Archivo demasiado grande.");

        // Save the file
        var uploadDirectory = Path.Combine(env.ContentRootPath, "uploads", "users");
        var fileName = $"{Guid.NewGuid()}{fileExtension}";
        var fullPath = Path.Combine(uploadDirectory, fileName);

        Directory.CreateDirectory(uploadDirectory);

        await using var stream = new FileStream(fullPath, FileMode.Create);
        await photo.CopyToAsync(stream, ct);

        var url = $"/uploads/users/{fileName}";

        return TypedResults.Ok(new { url });
    }
}