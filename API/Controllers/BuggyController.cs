
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    // BuggyController class derived from the BaseApiController class
    public class BuggyController : BaseApiController
    {
        // HTTP GET endpoint that returns a "Not Found" status
        [HttpGet("not-found")]
        public ActionResult GetNotFound()
        {
            // Returns a 404 Not Found status
            return NotFound();
        }

        // HTTP GET endpoint that returns a "Bad Request" status with a custom error message
        [HttpGet("bad-request")]
        public ActionResult GetBadRequest()
        {
            // Returns a 400 Bad Request status with a custom error message
            return BadRequest("This is a bad request");
        }

        // HTTP GET endpoint that intentionally throws an exception to simulate a server error
        [HttpGet("server-error")]
        public ActionResult GetServerError()
        {
            // Throws an exception to simulate a server error (status 500)
            throw new Exception("This is a server error");
        }

        // HTTP GET endpoint that returns an "Unauthorized" status
        [HttpGet("unauthorised")]
        public ActionResult GetUnauthorised()
        {
            // Returns a 401 Unauthorized status
            return Unauthorized();
        }
    }
}