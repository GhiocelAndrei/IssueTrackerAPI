using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using IssueTracker.Abstractions.Exceptions;

namespace IssueTrackerAPI.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class ErrorController : ControllerBase
    {
        [Route("/error")]
        public IActionResult HandleError()
        {
            var exception = HttpContext.Features.Get<IExceptionHandlerFeature>().Error;

            switch (exception)
            {
                case NotFoundException notFoundException:
                    return NotFound(notFoundException.Message);

                case InvalidInputException invalidInputException:
                    return BadRequest(invalidInputException.Message);

                case MissingValidatorException missingValidatorException:
                    return StatusCode(500, missingValidatorException.Message);

                case AuthenticationException authenticationException:
                    return BadRequest(authenticationException.Message);

                default:
                    return Problem();
            }            
        }

        [Route("/error-development")]
        public IActionResult HandleErrorDevelopment([FromServices] IHostEnvironment hostEnvironment)
        {
            if (!hostEnvironment.IsDevelopment())
            {
                return NotFound();
            }

            var exception = HttpContext.Features.Get<IExceptionHandlerFeature>().Error;

            switch (exception)
            {
                case NotFoundException notFoundException:
                    return NotFound(FormatedMessage(notFoundException.Message, notFoundException.StackTrace));

                case InvalidInputException invalidInputException:
                    return BadRequest(FormatedMessage(invalidInputException.Message, invalidInputException.StackTrace));

                case MissingValidatorException missingValidatorException:
                    return StatusCode(500, FormatedMessage(missingValidatorException.Message, missingValidatorException.StackTrace));

                case AuthenticationException authenticationException:
                    return BadRequest(FormatedMessage(authenticationException.Message, authenticationException.StackTrace));

                default:
                    return Problem(
                        detail: exception.StackTrace,
                        title: exception.Message);
            }
        }

        private string FormatedMessage(string Message, string StackTrace)
            => $"{Message} : {StackTrace}";
    }

}
