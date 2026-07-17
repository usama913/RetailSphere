using Microsoft.AspNetCore.Mvc;
using RetailSphere.Contracts.Common;
using RetailSphere.SharedKernel;

namespace RetailSphere.API.Common;

[ApiController]
public abstract class ApiControllerBase : ControllerBase
{
    /// <summary>
    /// Maps a Result/Result&lt;T&gt; (expected business outcome — §6/§7) to the
    /// standard ApiResponse envelope and the correct HTTP status for the error type.
    /// This is the only place that translation happens, so every endpoint behaves identically.
    /// </summary>
    protected IActionResult HandleResult<TValue>(Result<TValue> result)
    {
        if (result.IsSuccess)
            return Ok(ApiResponse<TValue>.Ok(result.Value));

        return MapError<TValue>(result.Error);
    }

    protected IActionResult HandleResult(Result result)
    {
        if (result.IsSuccess)
            return Ok(ApiResponse<object>.Ok(new { }));

        return MapError<object>(result.Error);
    }

    private IActionResult MapError<TValue>(Error error)
    {
        var apiError = new ApiError
        {
            Code = error.Code,
            Message = error.Message,
            TraceId = HttpContext.TraceIdentifier,
        };

        var response = ApiResponse<TValue>.Fail(apiError);

        return error.Type switch
        {
            ErrorType.Validation => BadRequest(response),
            ErrorType.NotFound => NotFound(response),
            ErrorType.Conflict => Conflict(response),
            ErrorType.Unauthorized => Unauthorized(response),
            _ => StatusCode(StatusCodes.Status500InternalServerError, response),
        };
    }
}
