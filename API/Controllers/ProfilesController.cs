using Application.Profiles;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class ProfilesController : BaseApiController
    {
        // Handles the HTTP GET request to retrieve a user's profile by username
        [HttpGet("{username}")]
        public async Task<IActionResult> GetProfile(string username)
        {
            // Send a query to retrieve details of a user's profile by username using Mediator
            return HandleResult(await Mediator.Send(new Details.Query { Username = username }));
        }
        // Handles the HTTP GET request to retrieve a user's activities by username and predicate
        [HttpGet("{username}/activities")]
        public async Task<IActionResult> GetUserActivities(string username, string predicate)
        {
            // Send a query to retrieve a list of user activities by username and predicate using Mediator
            return HandleResult(await Mediator.Send(new ListActivities.Query { Username = username, Predicate = predicate }));
        }
    }
}