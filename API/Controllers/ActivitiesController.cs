using Application.Activities;
using Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    // The ActivitiesController class derived from the BaseApiController class
    public class ActivitiesController : BaseApiController
    {
        // HTTP GET method representing an endpoint that lists all activities: api/activities
        [HttpGet] // api/activities
        public async Task<IActionResult> GetActivities()
        {
            // Send a request to the List.Query class in the application layer through the Mediator.Send() method
            return HandleResult(await Mediator.Send(new List.Query { }));
        }

        // HTTP GET method representing an endpoint that retrieves a specific activity: api/activities/{id}
        [HttpGet("{id}")] // api/activities/{id}
        public async Task<IActionResult> GetActicity(Guid id)
        {
            // Send a request with a specific ID to the Details.Query class in the application layer through the Mediator.Send() method
            return HandleResult(await Mediator.Send(new Details.Query { Id = id }));
        }

        // HTTP POST method representing an endpoint that creates a new activity
        [HttpPost]
        public async Task<IActionResult> CreateActivity([FromBody] Activity activity)
        {
            // Send a request with activity information to the Create.Command class in the application layer through the Mediator.Send() method
            return HandleResult(await Mediator.Send(new Create.Command { Activity = activity }));
        }

        // HTTP PUT method representing an endpoint that updates a specific activity
        [Authorize(Policy = "IsActivityHost")]
        [HttpPut("{id}")]
        public async Task<IActionResult> EditActivity(Guid id, [FromBody] Activity activity)
        {
            // Assign the activity ID and then send a request with the updated activity information to the Edit.Command class in the application layer through the Mediator.Send() method
            activity.Id = id;
            return HandleResult(await Mediator.Send(new Edit.Command { Activity = activity }));
        }

        // HTTP DELETE method representing an endpoint that deletes a specific activity
        [Authorize(Policy = "IsActivityHost")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            // Send a delete request with a specific ID to the Delete.Command class in the application layer through the Mediator.Send() method
            return HandleResult(await Mediator.Send(new Delete.Command { Id = id }));
        }

        // HTTP POST method representing an endpoint that updates attendance for a specific activity
        [HttpPost("{id}/attend")]
        public async Task<IActionResult> Attend(Guid id)
        {
            // Send an update request for attendance to the UpdateAttendance.Command class in the application layer through the Mediator.Send() method
            return HandleResult(await Mediator.Send(new UpdateAttendance.Command { Id = id }));
        }

    }
}