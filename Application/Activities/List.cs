using Application.Core;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Activities
{
    public class List
    {
        public class Query : IRequest<Result<List<Activity>>> { }
        // Handling the list operation based on the received query
        public class Handler : IRequestHandler<Query, Result<List<Activity>>>
        {
            private readonly DataContext _context;
            public Handler(DataContext context)
            {
                _context = context;
            }
            public async Task<Result<List<Activity>>> Handle(Query request, CancellationToken cancellationToken)
            {
                // Return a successful result containing the list of activities
                return Result<List<Activity>>.Success(await _context.Activities.ToListAsync());
            }
        }
    }
}