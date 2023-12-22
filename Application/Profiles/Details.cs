using Application.Core;
using Application.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Profiles
{
    public class Details
    {
        // Query class to request the details of a user profile
        public class Query : IRequest<Result<Profile>>
        {
            public string Username { get; set; }
        }
        // Handler class to handle the logic for retrieving user profile details
        public class Handler : IRequestHandler<Query, Result<Profile>>
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
            // Handle method to execute the logic for retrieving user profile details
            public async Task<Result<Profile>> Handle(Query request, CancellationToken cancellationToken)
            {
                // Retrieve the user profile details from the database based on the provided username
                var user = await _context.Users
                    .ProjectTo<Profile>(_mapper.ConfigurationProvider, new { currentUsername = _userAccessor.GetUserName() })
                    .SingleOrDefaultAsync(u => u.Username == request.Username);
                // If the user profile is not found, return a null result
                if (user == null) return null;
                // Return a success result with the retrieved user profile details
                return Result<Profile>.Success(user);
            }
        }
    }
}