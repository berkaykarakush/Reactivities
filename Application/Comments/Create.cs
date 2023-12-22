using System.Data;
using System.Runtime.CompilerServices;
using Application.Core;
using Application.Interfaces;
using AutoMapper;
using Domain;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Comments
{
    public class Create
    {
        public class Command : IRequest<Result<CommentDto>>
        {
            public string Body { get; set; }
            public Guid ActivityId { get; set; }
        }
        public class CommandValidator : AbstractValidator<CommentDto>
        {
            public CommandValidator()
            {
                // Comment body must not be empty
                RuleFor(c => c.Body).NotEmpty();
            }
        }
        public class Handler : IRequestHandler<Command, Result<CommentDto>>
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
            // Method handling the comment creation command
            public async Task<Result<CommentDto>> Handle(Command request, CancellationToken cancellationToken)
            {
                // Find the associated activity in the database
                var activity = await _context.Activities.FindAsync(request.ActivityId);
                // If the activity is not found, return null
                if(activity == null) return null;
                // Get the current user details from the database
                var user = await _context.Users
                    .Include(u => u.Photos)
                    .SingleOrDefaultAsync(u => u.UserName == _userAccessor.GetUserName());
                // If the user is not found, return null
                if(user == null) return null;
                // Create a new comment
                var comment = new Comment
                {
                    Author = user,
                    Activity = activity,
                    Body = request.Body
                };
                // Add the comment to the activity's comment collection
                activity.Comments.Add(comment);
                // Save changes to the database
                var success = await _context.SaveChangesAsync() > 0;
                // If the save is successful, return a success result with the mapped CommentDto
                // Kaydetme işlemi başarılı olursa, eşlenen CommentDto ile birlikte bir başarı sonucu döndürün
                if(success) return Result<CommentDto>.Success(_mapper.Map<CommentDto>(comment));
                // If there is a problem during saving, return a failure result with an error message
                return Result<CommentDto>.Failure("Failed to add comment");
            }
        }
    }
}