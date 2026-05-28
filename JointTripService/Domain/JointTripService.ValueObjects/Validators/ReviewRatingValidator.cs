using JointTripService.ValueObjects.Base;
using JointTripService.ValueObjects.Exceptions;

namespace JointTripService.ValueObjects.Validators;

public class ReviewRatingValidator : IValidator<int>
{
    public void Validate(int value)
    {
        if (value is < 1 or > 5)
            throw new InvalidReviewRatingException(value);
    }
}