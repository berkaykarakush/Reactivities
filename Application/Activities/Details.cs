using Application.Core;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Activities
{
    public class Details
    {
        // Query class representing the user's request to query details for an activity
        public class Query : IRequest<Result<ActivityDto>>
        {
            public Guid Id { get; set; }
        }
        // Handler class responsible for handling the query operation
        public class Handler : IRequestHandler<Query, Result<ActivityDto>>
        {
            private readonly DataContext _context;
            private readonly IMapper _mapper;
            public Handler(DataContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }
            // Handling the retrieval of activity details based on the received query
            public async Task<Result<ActivityDto>> Handle(Query request, CancellationToken cancellationToken)
            {
                // Find the activity with the requested identity value from the database
                var activity = await _context.Activities
                    .ProjectTo<ActivityDto>(_mapper.ConfigurationProvider) 
                    .FirstOrDefaultAsync(a => a.Id == request.Id);
                // If the activity is not found, return null
                if (activity == null) return null;
                // Return a successful result representing that the activity has been successfully found
                return Result<ActivityDto>.Success(activity);
            }
        }
    }
}