using Application.Core;
using Application.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Followers
{
    public class List
    {
        // Query class to request a list of follower or following profiles
        public class Query : IRequest<Result<List<Profiles.Profile>>>
        {
            public string Predicate { get; set; }
            public string Username { get; set; }
        }
        // Handler class to handle the logic for retrieving follower or following profiles
        public class Handler : IRequestHandler<Query, Result<List<Profiles.Profile>>>
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
            // Handle method to execute the logic for retrieving follower or following profiles
            public async Task<Result<List<Profiles.Profile>>> Handle(Query request, CancellationToken cancellationToken)
            {
                // Initialize a list to store the resulting profiles
                var profiles = new List<Profiles.Profile>();
                // Switch statement to determine whether to retrieve followers or following profiles
                switch (request.Predicate)
                {
                    // If the request is for followers, retrieve the follower profiles
                    case "followers":
                        profiles = await _context.UserFollowings
                            .Where(u => u.Target.UserName == request.Username)
                            .Select(u => u.Observer)
                            .ProjectTo<Profiles.Profile>(_mapper.ConfigurationProvider, new {currentUsername = _userAccessor.GetUserName()})
                            .ToListAsync();
                        break;
                    // If the request is for following, retrieve the following profiles
                    case "following":
                        profiles = await _context.UserFollowings
                        .Where(u => u.Observer.UserName == request.Username)
                        .Select(u => u.Target)
                        .ProjectTo<Profiles.Profile>(_mapper.ConfigurationProvider, new{currentUsername = _userAccessor.GetUserName()})
                        .ToListAsync();
                        break;
                }
                // Return a success result with the retrieved profiles
                return Result<List<Profiles.Profile>>.Success(profiles);
            }
        }
    }
}