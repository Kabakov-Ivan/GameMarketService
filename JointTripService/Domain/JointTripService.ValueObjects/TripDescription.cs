using JointTripService.ValueObjects.Base;
using JointTripService.ValueObjects.Validators;

namespace JointTripService.ValueObjects;

public class TripDescription(string value) : ValueObject<string>(new TripDescriptionValidator(), value.Trim());