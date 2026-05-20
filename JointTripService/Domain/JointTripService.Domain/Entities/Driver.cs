using JointTripService.Domain.Exceptions;
using JointTripService.ValueObjects;

namespace JointTripService.Domain.Entities;

public class Driver : User
{
    private readonly ICollection<Trip> _trips = [];

    public IReadOnlyCollection<Trip> Trips => _trips.ToList().AsReadOnly();

    protected Driver()
    {
    }

    public Driver(Guid id, FullName fullName, Email email) : base(id, fullName, email)
    {
    }

    public Trip CreateTrip(City origin, City destination, DateTime departureAt, SeatsCount seatsCount, string? description = null)
    {
        var trip = new Trip(this, origin, destination, departureAt, seatsCount, description);
        _trips.Add(trip);
        ModificationData = DateTime.UtcNow;
        return trip;
    }

    public bool EditTrip(Trip trip, City origin, City destination, DateTime departureAt, SeatsCount seatsCount, string? description = null)
    {
        if (trip == null)
            throw new ArgumentNullValueException(nameof(trip));

        if (trip.Driver != this)
            throw new AnotherUserEditTripException(trip, this);

        if (!_trips.Contains(trip))
            throw new AnotherUserEditTripException(trip, this);

        var isEdit = false;
        isEdit |= trip.ChangeOrigin(origin);
        isEdit |= trip.ChangeDestination(destination);
        isEdit |= trip.ChangeDepartureAt(departureAt);
        isEdit |= trip.ChangeSeatsCount(seatsCount);
        isEdit |= trip.ChangeDescription(description);

        if (isEdit)
            ModificationData = DateTime.UtcNow;

        return isEdit;
    }

    public bool CancelTrip(Trip trip)
    {
        if (trip == null)
            throw new ArgumentNullValueException(nameof(trip));

        if (trip.Driver != this)
            throw new AnotherUserEditTripException(trip, this);

        if (!_trips.Contains(trip))
            throw new AnotherUserEditTripException(trip, this);

        var isCancel = trip.Cancel();

        if (isCancel)
            ModificationData = DateTime.UtcNow;

        return isCancel;
    }

    public bool ConfirmBooking(Booking booking)
    {
        if (booking == null)
            throw new ArgumentNullValueException(nameof(booking));

        if (booking.Trip.Driver != this)
            throw new InvalidOperationException("Driver cannot approve booking for another driver trip");

        var isApproved = booking.Confirm();

        if (isApproved)
            ModificationData = DateTime.UtcNow;

        return isApproved;
    }

    public bool ApproveBooking(Booking booking)
        => ConfirmBooking(booking);

    public bool RejectBooking(Booking booking)
    {
        if (booking == null)
            throw new ArgumentNullValueException(nameof(booking));

        if (booking.Trip.Driver != this)
            throw new InvalidOperationException("Driver cannot reject booking for another driver trip");

        var isRejected = booking.RejectBooking();

        if (isRejected)
            ModificationData = DateTime.UtcNow;

        return isRejected;
    }
}