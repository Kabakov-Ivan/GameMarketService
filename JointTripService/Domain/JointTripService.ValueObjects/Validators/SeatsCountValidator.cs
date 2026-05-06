using JointTripService.ValueObjects.Base;
using JointTripService.ValueObjects.Exceptions;

namespace JointTripService.ValueObjects.Validators;

public class SeatsCountValidator : IValidator<int>
{
    public void Validate(int value)
    {
        if (value <= 0)
            throw new InvalidSeatsCountException(nameof(value));
    }
}