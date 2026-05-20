using JointTripService.ValueObjects.Exceptions;

namespace JointTripService.ValueObjects.Base;

public abstract class ValueObject<T> : IEquatable<ValueObject<T>>
{
    public T Value { get; }

    protected ValueObject(IValidator<T> validator, T value)
    {
        if (validator == null)
            throw new ValidatorNullException(nameof(validator));

        validator.Validate(value);
        Value = value;
    }

    public override string ToString()
        => Value?.ToString() ?? GetType().Name;

    public override int GetHashCode()
        => Value?.GetHashCode() ?? 0;

    public override bool Equals(object? other)
        => Equals(other as ValueObject<T>);

    public bool Equals(ValueObject<T>? other)
    {
        if (other is null)
            return false;

        if (ReferenceEquals(this, other))
            return true;

        if (GetType() != other.GetType())
            return false;

        return EqualityComparer<T>.Default.Equals(other.Value, Value);
    }

    public static bool operator ==(ValueObject<T>? left, ValueObject<T>? right)
        => Equals(left, right);

    public static bool operator !=(ValueObject<T>? left, ValueObject<T>? right)
        => !(left == right);
}