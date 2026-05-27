using JointTripService.ValueObjects.Base;
using JointTripService.ValueObjects.Validators;

namespace JointTripService.ValueObjects;

public class ReviewRating(int value) : ValueObject<int>(new ReviewRatingValidator(), value);