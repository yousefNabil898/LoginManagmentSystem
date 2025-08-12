using System.DataAcesses.Exceptions;

namespace LoginManagmentSystem
{
    public class CustomExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<CustomExceptionHandlerMiddleware> _logger;

        public CustomExceptionHandlerMiddleware(RequestDelegate next, ILogger<CustomExceptionHandlerMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);

                if (!httpContext.Response.HasStarted && httpContext.Response.StatusCode == StatusCodes.Status404NotFound)
                {
                    var error = new ErrorToReturn
                    {
                        statusCode = StatusCodes.Status404NotFound,
                        message = $"End point {httpContext.Request.Path} is not found"
                    };

                    httpContext.Response.ContentType = "application/json";
                    await httpContext.Response.WriteAsJsonAsync(error);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred");

                httpContext.Response.ContentType = "application/json";

                httpContext.Response.StatusCode = ex switch
                {
                    EmailAlreadyExistsException => StatusCodes.Status400BadRequest,
                    CompanyNotFoundException => StatusCodes.Status404NotFound,
                    OtpInvalidException => StatusCodes.Status400BadRequest,
                    OtpExpiredException => StatusCodes.Status400BadRequest,
                    NotFoundException => StatusCodes.Status404NotFound,
                    UnauthorizedException => StatusCodes.Status401Unauthorized,
                    BadRequestException => StatusCodes.Status400BadRequest,
                    _ => StatusCodes.Status500InternalServerError
                };

                var response = new ErrorToReturn
                {
                    statusCode = httpContext.Response.StatusCode,
                    message = ex.Message,
                    Errors = (ex is BadRequestException badEx) ? badEx.Errors : null
                };

                await httpContext.Response.WriteAsJsonAsync(response);
            }
        }
    }
}
