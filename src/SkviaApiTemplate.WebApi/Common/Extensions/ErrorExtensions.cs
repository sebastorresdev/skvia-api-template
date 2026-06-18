using ErrorOr;
using Microsoft.AspNetCore.Http.HttpResults;

namespace SkviaApiTemplate.WebApi.Common.Extensions;

public static class ErrorExtensions
{
    public static ProblemHttpResult ToProblem(this Error error)
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
