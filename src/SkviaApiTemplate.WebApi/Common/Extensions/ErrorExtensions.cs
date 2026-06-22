namespace SkviaApiTemplate.WebApi.Common.Extensions;

public static class ErrorExtensions
{
    extension(Error error)
    {
        public IResult ToProblem()
        {
            var statusCode = error.Type switch
            {
                ErrorType.Conflict => StatusCodes.Status409Conflict,
                ErrorType.Validation => StatusCodes.Status400BadRequest,
                ErrorType.NotFound => StatusCodes.Status404NotFound,
                ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
                ErrorType.Forbidden => StatusCodes.Status403Forbidden,
                _ => StatusCodes.Status500InternalServerError
            };

            return TypedResults.Problem(
                title: error.Code,
                detail: error.Description,
                statusCode: statusCode
            );
        }
    }

    extension(List<Error> errors)
    {
        public IResult ToProblem()
        {
            if(errors.Count is 0)
            {
                return TypedResults.Problem(
                    statusCode: StatusCodes.Status500InternalServerError,
                    title: "Ocurrió un error desconocido."
                );
            }
            
            // 1. Si todos los errores son de validación (FluentValidation)
            if(errors.All(e => e.Type == ErrorType.Validation))
            {
                var errorsDictionary = errors
                    .GroupBy(e => e.Code)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(e => e.Description).ToArray());

                return TypedResults.ValidationProblem(errorsDictionary);
            }
            
            var firstError = errors[0];
            
            return firstError.ToProblem();
        }
    }
}