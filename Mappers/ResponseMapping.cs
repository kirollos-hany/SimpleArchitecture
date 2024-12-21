using Microsoft.AspNetCore.Mvc;
using SimpleArchitecture.Common.Response;
using SimpleArchitecture.Common.Response.Enums;

namespace SimpleArchitecture.Mappers;

public static class ResponseMapping
{
    /// <summary>
    /// Maps failure response to the appropriate http response.
    /// </summary>
    /// <returns>An action result corresponding to the appropriate response.</returns>
    public static IResult ToResponse(this FailureResponse failureResponse)
    {
        return failureResponse.State switch
        {
            State.Unauthorized => TypedResults.Json(failureResponse.GetResponseData(),
                statusCode: StatusCodes.Status401Unauthorized),
            State.ValidationFailure => TypedResults.BadRequest(failureResponse.GetResponseData()),
            State.NotFound => TypedResults.NotFound(failureResponse.GetResponseData()),
            State.InternalError => TypedResults.Json(failureResponse.GetResponseData(),
                statusCode: StatusCodes.Status500InternalServerError),
            State.TooManyRequests => TypedResults.Json(failureResponse.GetResponseData(),
                statusCode: StatusCodes.Status429TooManyRequests),
            _ => throw new ArgumentException("Response state not supported.")
        };
    }
}