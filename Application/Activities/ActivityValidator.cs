using Domain;
using FluentValidation;

namespace Application.Activities
{
    // ActivityValidator class that defines validation rules for the Activity class
    public class ActivityValidator : AbstractValidator<Activity>
    {
        // Constructor where validation rules are specified
        public ActivityValidator()
        {
            // Validation rule for the Title property of the Activity class, ensuring it is not empty
            RuleFor(a => a.Title).NotEmpty();
            // Validation rule for the Description property of the Activity class, ensuring it is not empty
            RuleFor(a => a.Description).NotEmpty();
            // Validation rule for the Date property of the Activity class, ensuring it is not empty
            RuleFor(a => a.Date).NotEmpty();
            // Validation rule for the Category property of the Activity class, ensuring it is not empty
            RuleFor(a => a.Category).NotEmpty();
            // Validation rule for the City property of the Activity class, ensuring it is not empty
            RuleFor(a => a.City).NotEmpty();
            // Validation rule for the Venue property of the Activity class, ensuring it is not empty
            RuleFor(a => a.Venue).NotEmpty();
        }
    }
}