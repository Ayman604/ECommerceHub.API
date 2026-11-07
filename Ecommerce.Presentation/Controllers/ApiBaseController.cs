using ECommerce.Shared.CommonResponses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Security.Claims;

namespace ECommerce.Presentation.Controllers
{
    [ApiController]
    [Route(template: "api/[Controller]")]
    public class ApiBaseController : ControllerBase
    {
        protected IActionResult HandleResult(Result result)
        {
            if (result.IsSuccess)
                return NoContent();
            else
                return HandleProblem(result.Errors);
        }
        protected string GetEmailFromToken() => User.FindFirstValue(ClaimTypes.Email)!;
        protected ActionResult<TValue> HandleResult<TValue>(Result<TValue> result)
        {
            if (result.IsSuccess)
                return Ok(result.Value);
            else
                return HandleProblem(result.Errors);
        }
        private ActionResult HandleProblem(IReadOnlyList<Error> errors)
        {
            if (errors.Count == 0)
            {
                return Problem(
                    statusCode: StatusCodes.Status500InternalServerError,
                    title: "An Error Occured"
                    );
            }
            if (errors.All(E => E.ErrorType == ErrorType.Validation))
                return HandleValidationErrors(errors);
            return HandleSingleError(errors[0]);
        }
        private ActionResult HandleSingleError(Error error)
        {
            return Problem(
                title: error.Code,
                detail: error.Description,
                type: error.ErrorType.ToString(),
                statusCode: MapErrorTypeIntoStatusCode(error.ErrorType)
                );

        }
        private ActionResult HandleValidationErrors(IReadOnlyList<Error> errors)
        {
            var modelState = new ModelStateDictionary();
            foreach (var error in errors)
            {
                modelState.AddModelError(error.Code, error.Description);
            }
            return ValidationProblem(modelState);
        }
        private static int MapErrorTypeIntoStatusCode(ErrorType errorType) =>
            errorType switch
            {
                ErrorType.NotFound => StatusCodes.Status404NotFound,
                ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
                ErrorType.Forbidden => StatusCodes.Status403Forbidden,
                ErrorType.Validation => StatusCodes.Status400BadRequest,
                ErrorType.InvalidCredentials => StatusCodes.Status401Unauthorized,
                _ => StatusCodes.Status500InternalServerError,
            };
    }
}
