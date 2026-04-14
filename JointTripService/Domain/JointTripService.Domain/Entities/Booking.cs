using JointTripService.Domain.Base;
using JointTripService.Domain.Enums;
using JointTripService.Domain.Exceptions;

namespace JointTripService.Domain.Entities;

public class Booking : Entity<Guid>
{
    public User Passenger { get; private set; } = default!;
    public Trip Trip { get; private set; } = default!;
    public int SeatsCount { get; private set; }
    public BookingStatus Status { get; private set; }
    public DateTime CreationData { get; }
    public DateTime? ModificationData { get; private set; }

    protected Booking()
    {
    }

    public Booking(User passenger, Trip trip, int seatsCount)
        : this(Guid.NewGuid(), passenger, trip, seatsCount)
    {
    }

    protected Booking(Guid id, User passenger, Trip trip, int seatsCount)
        : base(id)
    {
        if (id == Guid.Empty)
            throw new InvalidIdException();

        Passenger = passenger ?? throw new ArgumentNullValueException(nameof(passenger));
        Trip = trip ?? throw new ArgumentNullValueException(nameof(trip));

        if (Passenger == Trip.Driver)
            throw new TripCannotBeBookedByDriverException(trip);

        if (seatsCount <= 0)
            throw new ArgumentOutOfRangeException(nameof(seatsCount));

        if (Trip.Status != TripStatus.Published)
            throw new TripCannotBeBookedException(trip);

        SeatsCount = seatsCount;
        Status = BookingStatus.Pending;
        CreationData = DateTime.UtcNow;

        Trip.AddBooking(this);
    }

    public bool Approve()
    {
        if (Status != BookingStatus.Pending)
            throw new BookingCannotBeApprovedException(this);

        if (Trip.Status != TripStatus.Published)
            throw new BookingCannotBeApprovedException(this);

        if (Trip.AvailableSeats < SeatsCount)
            throw new TripHasNoAvailableSeatsException(Trip);

        Trip.BookSeats(SeatsCount);
        Status = BookingStatus.Approved;
        return SetModificationData(DateTime.UtcNow);
    }

    public bool Reject()
    {
        if (Status != BookingStatus.Pending)
            throw new BookingCannotBeRejectedException(this);

        Status = BookingStatus.Rejected;
        return SetModificationData(DateTime.UtcNow);
    }

    public bool Cancel()
    {
        if (Status == BookingStatus.Cancelled)
            throw new BookingCannotBeCancelledException("The booking is already cancelled");

        if (Status == BookingStatus.Rejected)
            throw new BookingCannotBeCancelledException("The booking is already rejected");

        if (Status == BookingStatus.Approved)
            Trip.ReleaseSeats(SeatsCount);

        Status = BookingStatus.Cancelled;
        return SetModificationData(DateTime.UtcNow);
    }

    private bool SetModificationData(DateTime modificationData)
    {
        if (ModificationData == null && modificationData < CreationData)
            throw new InvalidModificationDataException(this, modificationData);

        if (ModificationData != null && modificationData < ModificationData)
            throw new InvalidModificationDataException(this, modificationData);

        if (ModificationData == modificationData)
            return false;

        ModificationData = modificationData;
        return true;
    }
}