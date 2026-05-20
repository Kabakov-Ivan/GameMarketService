using JointTripService.Domain.Exceptions;
using JointTripService.ValueObjects;

namespace JointTripService.Domain.Entities;

public class Passenger : User
{
    private readonly ICollection<Booking> _bookings = [];

    public IReadOnlyCollection<Booking> Bookings => _bookings.ToList().AsReadOnly();

    protected Passenger()
    {
    }

    public Passenger(Guid id, FullName fullName, Email email) : base(id, fullName, email)
    {
    }

    public Booking BookTrip(Trip trip, SeatsCount seatsCount)
    {
        if (trip == null)
            throw new ArgumentNullValueException(nameof(trip));

        if (trip.Driver.Email == Email)
            throw new TripCannotBeBookedByDriverException(trip);

        var booking = new Booking(this, trip, seatsCount);
        _bookings.Add(booking);
        ModificationData = DateTime.UtcNow;
        return booking;
    }

    public Booking CreateBooking(Trip trip, SeatsCount seatsCount)
        => BookTrip(trip, seatsCount);
}