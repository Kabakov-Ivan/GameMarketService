using JointTripService.ValueObjects.Base;
using JointTripService.ValueObjects.Validators;

namespace JointTripService.ValueObjects;

public class SeatsCount(int value) : ValueObject<int>(new SeatsCountValidator(), value)
{
    public static SeatsCount operator +(SeatsCount left, SeatsCount right)
        => new(left.Value + right.Value);

    public static SeatsCount operator -(SeatsCount left, SeatsCount right)
        => new(left.Value - right.Value);

    public static bool operator >(SeatsCount left, SeatsCount right)
        => left.Value > right.Value;

    public static bool operator <(SeatsCount left, SeatsCount right)
        => left.Value < right.Value;

    public static bool operator >=(SeatsCount left, SeatsCount right)
        => left.Value >= right.Value;

    public static bool operator <=(SeatsCount left, SeatsCount right)
        => left.Value <= right.Value;
}