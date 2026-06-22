using Microsoft.AspNetCore.Authentication.JwtBearer;
using SkviaApiTemplate.WebApi.Auth;
using SkviaApiTemplate.WebApi.Auth.Security;
using SkviaApiTemplate.WebApi.Auth.Services;
using SkviaApiTemplate.WebApi.Common.Exceptions;
using SkviaApiTemplate.WebApi.Common.OpenApi;
using SkviaApiTemplate.WebApi.Persistence.Interceptors;

namespace SkviaApiTemplate.WebApi;

public static class DependencyInjection
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddWebApi(IConfiguration configuration)
        {
            // Esto habilita el IHttpContextAccessor que inyectamos en el constructor del servicio
            services.AddHttpContextAccessor();

            // FluentValidation
            services.AddValidatorsFromAssemblyContaining<Program>();

            // Errors
            services.AddExceptionHandler<GlobalExceptionHandler>();
            services.AddProblemDetails();

            // CORS
            services.AddCors(opt =>
            {
                opt.AddPolicy("AllowAll", policy =>
                    policy.AllowAnyOrigin()
                          .AllowAnyHeader()
                          .AllowAnyMethod());
            });

            // Auth
            services.AddScoped<IPasswordService,PasswordService>();
            services.AddScoped<CurrentUser>();

            // DB
            services.AddScoped<AuditableEntityInterceptor>();
            services.AddDbContext<AppDbContext>((sp, opt) =>
            {
                var interceptor = sp.GetRequiredService<AuditableEntityInterceptor>();

                opt.UseNpgsql(configuration.GetConnectionString("Default"))
                   .AddInterceptors(interceptor);
                opt.UseSnakeCaseNamingConvention();
            });

            // OpenAPI
            services.AddOpenApi(opt =>
            {
                opt.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
            });

            // JWT
            services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.Section));
            services.ConfigureOptions<JwtBearerTokenValidationConfiguration>();
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer();
            services.AddSingleton<IJwtService, JwtService>();

            services.AddAuthorization();

            return services;
        }
    }
}
