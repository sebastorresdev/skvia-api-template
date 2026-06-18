namespace SkviaApiTemplate.WebApi.Common.Filters;

public class ValidationFilter<TRequest>(IValidator<TRequest> validator) : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(
        EndpointFilterInvocationContext context,
        EndpointFilterDelegate next)
    {
        var request = context.Arguments
            .OfType<TRequest>()
            .FirstOrDefault();

        if (request is null)
            return await next(context);

        var validationResult = await validator.ValidateAsync(request, context.HttpContext.RequestAborted);

        if (validationResult.IsValid) return await next(context);
        // Convertir los errores de FluentValidation a errores de ErrorOr
        var errors = validationResult.Errors
            .ConvertAll(error => Error.Validation(
                code: error.PropertyName,
                description: error.ErrorMessage));

        // Devolver el primer error usando nuestro formato estandarizado
        // (ErrorOr también tiene un método para devolver múltiples errores, pero
        // para mantenerlo simple y consistente con los otros errores, devolvemos el primero)
        return errors.First().ToProblem();

    }
}
