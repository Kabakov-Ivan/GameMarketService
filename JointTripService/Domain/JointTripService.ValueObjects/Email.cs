using JointTripService.ValueObjects.Base;
using JointTripService.ValueObjects.Validators;

namespace JointTripService.ValueObjects;

public class Email(string value) : ValueObject<string>(new EmailValidator(), Normalize(value))
{
    private static string Normalize(string value)
        => value.Trim().ToLowerInvariant();
}