using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Core;
using AutoMapper;
using Domain;
using FluentValidation;
using MediatR;
using Persistence;

namespace Application.Activities
{
    public class Edit
    {
        // Command class representing the request for editing: Command
        public class Command : IRequest<Result<Unit>>
        {
            public Activity Activity { get; set; }
        }
        // Validator class for validating the Edit Command: CommandValidator
        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                // Setting up a validation rule using the ActivityValidator for the Activity property
                RuleFor(e => e.Activity).SetValidator(new ActivityValidator());
            }
        }
        // Handler class responsible for handling the edit operation: Handler
        public class Handler : IRequestHandler<Command, Result<Unit>>
        {
            private readonly DataContext _context;
            private readonly IMapper _mapper;

            public Handler(DataContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }
            // Handling the edit operation based on the received command
            public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
            {
                // Find the activity with the requested identity value from the database
                var activity = await _context.Activities.FindAsync(request.Activity.Id);
                // If the activity is not found, return null
                if (activity == null) return null;
                // Map the incoming Activity object to the existing activity
                _mapper.Map(request.Activity, activity);
                // Save changes to the database and check if any changes were made
                var result = await _context.SaveChangesAsync() > 0;
                // Return success or failure result based on the database operation
                if (!result) return Result<Unit>.Failure("Failed to update activity");
                return Result<Unit>.Success(Unit.Value);
            }
        }
    }
}