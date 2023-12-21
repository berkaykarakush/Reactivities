using Application.Comments;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace API.SignalR
{
    public class ChatHub : Hub
    {
        private readonly IMediator _mediator;

        public ChatHub(IMediator mediator)
        {
            _mediator = mediator;
        }
        // Method to handle sending a comment through SignalR
        public async Task SendComment(Create.Command command)
        {
            // Send the comment creation command and get the result
            var comment = await _mediator.Send(command);
            // Send the newly created comment to the clients in the specified group
            await Clients.Group(command.ActivityId.ToString()).SendAsync("ReceiveComment", comment.Value);
        }
        // Method called when a client is connected to the hub
        public override async Task OnConnectedAsync()
        {
            // Get the HttpContext from the SignalR context
            var httpContext = Context.GetHttpContext();
            // Extract the activityId from the query parameters
            var activityId = httpContext.Request.Query["activityId"];
            // Add the connected client to the group associated with the activityId
            await Groups.AddToGroupAsync(Context.ConnectionId, activityId);
            // Send the existing comments for the activity to the connected client
            var result = await _mediator.Send(new List.Query { ActivityId = Guid.Parse(activityId) });
            await Clients.Caller.SendAsync("LoadComments", result.Value);

        }
    }
}