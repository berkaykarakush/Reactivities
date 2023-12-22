using Application.Core;
using Application.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Persistence;

namespace Application.Activities
{
    public class List
    {
        // Represents the query to retrieve a paged list of ActivityDto
        public class Query : IRequest<Result<PagedList<ActivityDto>>>
        {
            public ActivityParams Params { get; set; }
        }
        // Handling the list operation based on the received query
        public class Handler : IRequestHandler<Query, Result<PagedList<ActivityDto>>>
        {
            private readonly DataContext _context;
            private readonly IMapper _mapper;
            private readonly IUserAccessor _userAccessor;
            public Handler(DataContext context, IMapper mapper, IUserAccessor userAccessor)
            {
                _context = context;
                _mapper = mapper;
                _userAccessor = userAccessor;
            }
            public async Task<Result<PagedList<ActivityDto>>> Handle(Query request, CancellationToken cancellationToken)
            {
                // Query the activities from the database and project them to ActivityDto using AutoMapper
                var query = _context.Activities
                    .Where(d => d.Date >= request.Params.StartDate)
                    .OrderBy(d => d.Date)
                    .ProjectTo<ActivityDto>(_mapper.ConfigurationProvider, new { currentUsername = _userAccessor.GetUserName() })
                    .AsQueryable();
                // Filter activities based on user participation and hosting criteria
                if (request.Params.IsGoing && !request.Params.IsHost)
                {
                    query = query.Where(q => q.Attendees.Any(a => a.Username == _userAccessor.GetUserName()));
                }
                if (request.Params.IsHost && !request.Params.IsGoing)
                {
                    query = query.Where(q => q.HostUsername == _userAccessor.GetUserName());
                }
                // Return a successful result containing the list of activities
                return Result<PagedList<ActivityDto>>.Success(
                    await PagedList<ActivityDto>.CreateAsync(query, request.Params.pageNumber, request.Params.PageSize)
                );
            }
        }
    }
}