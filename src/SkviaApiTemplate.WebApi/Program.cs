using SkviaApiTemplate.WebApi;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    Log.Information("Iniciando el servidor web de la API...");

    var builder = WebApplication.CreateBuilder(args);

    builder.Services.AddSerilog((services, loggerConfiguration) => loggerConfiguration
        .ReadFrom.Configuration(builder.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext());

    builder.Services
        .AddWebApi(builder.Configuration);

    var app = builder.Build();

    await app.AddConfigAsync();

    app.Run();

}
catch(Exception ex)
{
    Log.Fatal(ex, "El host de la API terminó inesperadamente.");
    return 1;
}
finally
{
    Log.CloseAndFlush();
}

return 0;