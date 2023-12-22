using Application.Core;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Comments
{
    public class List
    {
        // Query class to request a list of comments for a specific activity
        public class Query : IRequest<Result<List<CommentDto>>>
        {
            public Guid ActivityId { get; set; }
        }
        // Handler class to handle the logic for retrieving comments for a specific activity
        public class Handler : IRequestHandler<Query, Result<List<CommentDto>>>
        {
            private readonly DataContext _context;
            private readonly IMapper _mapper;

            public Handler(DataContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }
            // Handle method to execute the logic for retrieving comments for a specific activity
            public async Task<Result<List<CommentDto>>> Handle(Query request, CancellationToken cancellationToken)
            {
                // Retrieve the comments for the specified activity from the database
                var comments = await _context.Comments
                    .Where(c => c.Activity.Id == request.ActivityId)
                    .OrderBy(o => o.CreatedDate)
                    .ProjectTo<CommentDto>(_mapper.ConfigurationProvider)
                    .ToListAsync();
                // Return a success result with the retrieved comments
                return Result<List<CommentDto>>.Success(comments);
            }
        }
    }
}