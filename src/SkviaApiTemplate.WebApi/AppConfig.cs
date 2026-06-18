using Scalar.AspNetCore;

namespace SkviaApiTemplate.WebApi;

public static class AppConfig
{
    extension(WebApplication app)
    {
        public async Task AddConfigAsync()
        {
            // Configure the HTTP request pipeline.
            if(app.Environment.IsDevelopment())
            {
                app.MapOpenApi("/api/{documentName}/openapi.json");

                app.MapScalarApiReference(options =>
                {
                    options
                    .WithTitle("SKVIA — API Docs")
                    .WithTheme(ScalarTheme.Saturn)
                    .AddDocument("v1", "Versión 1", routePattern: "/api/v1/openapi.json")
                    .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);

                });
            }

            await app.InitializeDatabaseAsync();

            app.UseCors("AllowAll");
            app.UseExceptionHandler();

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(
                Path.Combine(app.Environment.ContentRootPath, "uploads")),
                RequestPath = "/uploads"
            });

            app.UseAuthentication();
            app.UseAuthorization();

            app.Map("/", () => Results.Redirect("/scalar"));

            app.MapEndpoints(typeof(Program).Assembly);
        }
    }
}
