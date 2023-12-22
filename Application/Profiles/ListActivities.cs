using Application.Core;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Profiles
{
    public class ListActivities
    {
        public class Query : IRequest<Result<List<UserActivityDto>>>
        {
            public string Username { get; set; }
            public string Predicate { get; set; }
        }
        public class Handler : IRequestHandler<Query, Result<List<UserActivityDto>>>
        {
            private readonly DataContext _context;
            private readonly IMapper _mapper;

            public Handler(DataContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<Result<List<UserActivityDto>>> Handle(Query request, CancellationToken cancellationToken)
            {
                // Query to retrieve activity attendees from the database and project them to UserActivityDto using AutoMapper
                var query = _context.ActivityAttendees
                    .Where(q => q.AppUser.UserName == request.Username)
                    .OrderBy(a => a.Activity.Date)
                    .ProjectTo<UserActivityDto>(_mapper.ConfigurationProvider)
                    .AsQueryable();
                // Apply additional filtering based on the specified predicate
                query = request.Predicate switch
                {
                    "past" => query.Where(q => q.Date <= DateTime.Now),
                    "hosting" => query.Where(q => q.HostUsername == request.Username),
                    _ => query.Where(q => q.Date >= DateTime.Now)
                };
                // Retrieve and return the list of user activities
                var activities = await query.ToListAsync();
                return Result<List<UserActivityDto>>.Success(activities);
            }
        }
    }
}