using Application.Core;
using MediatR;
using Persistence;

namespace Application.Activities
{
    public class Delete
    {
        // Command class representing the request for deletion: Command
        public class Command : IRequest<Result<Unit>>
        {
            public Guid Id { get; set; }
        }
        // Handler class responsible for handling the delete operation: Handler
        public class Handler : IRequestHandler<Command, Result<Unit>>
        {
            private readonly DataContext _context;

            public Handler(DataContext context)
            {
                _context = context;
            }
            // Handling the delete operation based on the received command
            public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
            {
                // Find the activity with the requested identity value from the database
                var activity = await _context.Activities.FindAsync(request.Id);
                // If the activity is not found, return null
                if (activity == null) return null;
                // Remove the activity from the context
                _context.Remove(activity);
                // Save changes to the database and check if any changes were made
                var result = await _context.SaveChangesAsync() > 0;
                // Return success or failure result based on the database operation
                if (!result) return Result<Unit>.Failure("Failed to delete the activity");
                return Result<Unit>.Success(Unit.Value);
            }
        }

    }
}