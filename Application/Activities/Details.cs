using Application.Core;
using Domain;
using MediatR;
using Persistence;

namespace Application.Activities
{
    public class Details
    {
        // Query class representing the user's request to query details for an activity
        public class Query : IRequest<Result<Activity>>
        {
            public Guid Id { get; set; }
        }
        // Handler class responsible for handling the query operation
        public class Handler : IRequestHandler<Query, Result<Activity>>
        {
            private readonly DataContext _context;

            public Handler(DataContext context)
            {
                _context = context;
            }
            // Handling the retrieval of activity details based on the received query
            public async Task<Result<Activity>> Handle(Query request, CancellationToken cancellationToken)
            {
                // Find the activity with the requested identity value from the database
                var activity = await _context.Activities.FindAsync(request.Id);
                // If the activity is not found, return null
                if (activity == null) return null;
                // Return a successful result representing that the activity has been successfully found
                return Result<Activity>.Success(activity);
            }
        }
    }
}