using JointTripService.ValueObjects.Base;
using JointTripService.ValueObjects.Validators;

namespace JointTripService.ValueObjects;

public class FullName(string value) : ValueObject<string>(new FullNameValidator(), value.Trim());