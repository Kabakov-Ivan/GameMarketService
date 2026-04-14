using JointTripService.ValueObjects.Base;
using JointTripService.ValueObjects.Validators;

namespace JointTripService.ValueObjects;

public class ReviewText(string value) : ValueObject<string>(new ReviewTextValidator(), value.Trim());