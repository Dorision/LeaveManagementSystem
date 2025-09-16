using FluentValidation;
using LeaveManagementSystem.DTOs;

namespace LeaveManagementSystem.Validators;

public class CreateLeaveRequestDtoValidator : AbstractValidator<CreateLeaveRequestDto>
{
    public CreateLeaveRequestDtoValidator()
    {
        RuleFor(x => x.StartDate)
            .NotEmpty().WithMessage("Start date is required")
            .Must(date => date.Date >= DateTime.Today).WithMessage("Start date must be today or in the future");

        RuleFor(x => x.EndDate)
            .NotEmpty().WithMessage("End date is required")
            .Must((dto, endDate) => endDate.Date >= dto.StartDate.Date)
            .WithMessage("End date must be equal to or after start date");

        RuleFor(x => x.Comment)
            .MaximumLength(500).WithMessage("Comment cannot exceed 500 characters");
    }
}

public class UpdateLeaveRequestDtoValidator : AbstractValidator<UpdateLeaveRequestDto>
{
    public UpdateLeaveRequestDtoValidator()
    {
        RuleFor(x => x.StartDate)
            .NotEmpty().WithMessage("Start date is required")
            .Must(date => date.Date >= DateTime.Today).WithMessage("Start date must be today or in the future");

        RuleFor(x => x.EndDate)
            .NotEmpty().WithMessage("End date is required")
            .Must((dto, endDate) => endDate.Date >= dto.StartDate.Date)
            .WithMessage("End date must be equal to or after start date");

        RuleFor(x => x.Comment)
            .MaximumLength(500).WithMessage("Comment cannot exceed 500 characters");
    }
}

public class LeaveRequestDecisionDtoValidator : AbstractValidator<LeaveRequestDecisionDto>
{
    public LeaveRequestDecisionDtoValidator()
    {
        RuleFor(x => x.Comment)
            .NotEmpty().WithMessage("Comment is required when making a decision")
            .MaximumLength(500).WithMessage("Comment cannot exceed 500 characters");
    }
}