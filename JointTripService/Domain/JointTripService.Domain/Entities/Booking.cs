using JointTripService.Domain.Base;
using JointTripService.Domain.Enums;
using JointTripService.Domain.Exceptions;
using JointTripService.ValueObjects;

namespace JointTripService.Domain.Entities;

public class Booking : Entity<Guid>
{
    public Passenger Passenger { get; private set; } = default!;
    public Trip Trip { get; private set; } = default!;
    public Driver? ConfirmedByDriver { get; private set; }
    public Driver? RejectedByDriver { get; private set; }
    public SeatsCount SeatsCount { get; private set; } = default!;
    public BookingStatus Status { get; private set; }
    public DateTime CreationData { get; }
    public DateTime? ModificationData { get; private set; }

    protected Booking()
    {
    }

    public Booking(Passenger passenger, Trip trip, SeatsCount seatsCount, DateTime? creationData = null, DateTime? modificationData = null)
        : this(Guid.NewGuid(), passenger, trip, seatsCount, BookingStatus.Pending, creationData ?? DateTime.UtcNow, modificationData, null, null)
    {
    }

    protected Booking(Guid id, Passenger passenger, Trip trip, SeatsCount seatsCount, BookingStatus status, DateTime creationData, DateTime? modificationData, Driver? confirmedByDriver, Driver? rejectedByDriver)
        : base(id)
    {
        if (id == Guid.Empty)
            throw new InvalidIdException();

        Passenger = passenger ?? throw new ArgumentNullValueException(nameof(passenger));
        Trip = trip ?? throw new ArgumentNullValueException(nameof(trip));

        if (Passenger.Email == Trip.Driver.Email)
            throw new TripCannotBeBookedByDriverException(trip);

        if (seatsCount == null)
            throw new ArgumentNullValueException(nameof(seatsCount));

        if (Trip.Status != TripStatus.Published && status == BookingStatus.Pending)
            throw new TripCannotBeBookedException(trip);

        SeatsCount = seatsCount;
        Status = status;
        CreationData = creationData;
        ModificationData = modificationData;
        ConfirmedByDriver = confirmedByDriver;
        RejectedByDriver = rejectedByDriver;
    }

    internal bool Confirm(Driver confirmer)
    {
        if (confirmer == null)
            throw new ArgumentNullValueException(nameof(confirmer));

        if (Trip.Driver != confirmer)
            throw new DriverCannotManageAnotherDriversBookingException(confirmer, this);

        if (Status != BookingStatus.Pending)
            throw new BookingCannotBeApprovedException(this);

        if (Trip.Status != TripStatus.Published)
            throw new BookingCannotBeApprovedException(this);

        if (Trip.AvailableSeats < SeatsCount.Value)
            throw new TripHasNoAvailableSeatsException(Trip);

        Trip.BookSeats(SeatsCount);
        ConfirmedByDriver = confirmer;
        Status = BookingStatus.Approved;
        return SetModificationData(DateTime.UtcNow);
    }

    internal bool RejectBooking(Driver rejecter)
    {
        if (rejecter == null)
            throw new ArgumentNullValueException(nameof(rejecter));

        if (Trip.Driver != rejecter)
            throw new DriverCannotManageAnotherDriversBookingException(rejecter, this);

        if (Status != BookingStatus.Pending)
            throw new BookingCannotBeRejectedException(this);

        RejectedByDriver = rejecter;
        Status = BookingStatus.Rejected;
        return SetModificationData(DateTime.UtcNow);
    }

    public bool Cancel()
    {
        if (Status == BookingStatus.Cancelled)
            throw new BookingCannotBeCancelledException("Бронирование уже отменено");

        if (Status == BookingStatus.Rejected)
            throw new BookingCannotBeCancelledException("Бронирование уже отклонено");

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