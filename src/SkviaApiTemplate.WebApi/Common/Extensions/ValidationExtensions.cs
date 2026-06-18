using SkviaApiTemplate.WebApi.Common.Filters;

namespace SkviaApiTemplate.WebApi.Common.Extensions;

public static class ValidationExtensions
{
    extension(RouteHandlerBuilder builder)
    {
        public RouteHandlerBuilder WithRequestValidation<TRequest>()
            => builder.AddEndpointFilter<ValidationFilter<TRequest>>()
                .ProducesValidationProblem();
    }
}
