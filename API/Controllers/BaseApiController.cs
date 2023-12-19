using Application.Core;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BaseApiController : ControllerBase
    {
        private IMediator _mediator;
        protected IMediator Mediator => _mediator ??= HttpContext.RequestServices.GetService<IMediator>();
        // Method to handle the result of a MediatR operation
        protected ActionResult HandleResult<T>(Result<T> result)
        {
            // If the result is null, return a 404 Not Found status
            if (result == null) return NotFound();
            // If the operation is successful and the result has a value, return a 200 OK status with the result value
            if (result.IsSucces && result.Value != null)
                return Ok(result.Value);
            // If the operation is successful but the result value is null, return a 404 Not Found status
            if (result.IsSucces && result.Value == null)
                return NotFound();
            // If the operation is not successful, return a 400 Bad Request status with the error message
            return BadRequest(result.Error);
        }
    }
}