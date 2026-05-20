using JointTripService.Domain.Base;
using JointTripService.Domain.Exceptions;
using JointTripService.Domain.Enums;
using JointTripService.ValueObjects;

namespace JointTripService.Domain.Entities;

public class Trip : Entity<Guid>
{
    private readonly ICollection<Booking> _bookings = [];

    public Driver Driver { get; private set; } = default!;
    public City Origin { get; private set; } = default!;
    public City Destination { get; private set; } = default!;
    public DateTime DepartureAt { get; private set; }
    public SeatsCount SeatsCount { get; private set; } = default!;
    public int AvailableSeats { get; private set; }
    public string? Description { get; private set; }
    public TripStatus Status { get; private set; }
    public DateTime CreationData { get; }
    public DateTime? ModificationData { get; private set; }

    public IReadOnlyCollection<Booking> Bookings => _bookings.ToList().AsReadOnly();

    protected Trip()
    {
    }

    public Trip(Driver driver, City origin, City destination, DateTime departureAt, SeatsCount seatsCount, string? description = null)
        : this(Guid.NewGuid(), driver, origin, destination, departureAt, seatsCount, description)
    {
    }

    protected Trip(Guid id, Driver driver, City origin, City destination, DateTime departureAt, SeatsCount seatsCount, string? description = null)
        : base(id)
    {
        if (id == Guid.Empty)
            throw new InvalidIdException();

        Driver = driver ?? throw new ArgumentNullValueException(nameof(driver));
        Origin = origin ?? throw new ArgumentNullValueException(nameof(origin));
        Destination = destination ?? throw new ArgumentNullValueException(nameof(destination));
        SeatsCount = seatsCount ?? throw new ArgumentNullValueException(nameof(seatsCount));

        if (Origin == Destination)
            throw new ArgumentException("Origin and destination must be different");

        DepartureAt = departureAt;
        AvailableSeats = seatsCount.Value;
        Description = description?.Trim();
        Status = TripStatus.Draft;
        CreationData = DateTime.UtcNow;
    }

    public bool ChangeOrigin(City newOrigin)
    {
        EnsureEditable();

        if (newOrigin == null)
            throw new ArgumentNullValueException(nameof(newOrigin));

        if (Origin == newOrigin)
            return false;

        Origin = newOrigin;
        return SetModificationData(DateTime.UtcNow);
    }

    public bool ChangeDestination(City newDestination)
    {
        EnsureEditable();

        if (newDestination == null)
            throw new ArgumentNullValueException(nameof(newDestination));

        if (Destination == newDestination)
            return false;

        Destination = newDestination;
        return SetModificationData(DateTime.UtcNow);
    }

    public bool ChangeDepartureAt(DateTime newDepartureAt)
    {
        EnsureEditable();

        if (DepartureAt == newDepartureAt)
            return false;

        DepartureAt = newDepartureAt;
        return SetModificationData(DateTime.UtcNow);
    }

    public bool ChangeSeatsCount(SeatsCount newSeatsCount)
    {
        EnsureEditable();

        if (newSeatsCount == null)
            throw new ArgumentNullValueException(nameof(newSeatsCount));

        if (SeatsCount == newSeatsCount)
            return false;

        var reservedSeats = SeatsCount.Value - AvailableSeats;

        if (newSeatsCount.Value < reservedSeats)
            throw new TripCannotBeEditedException(this);

        SeatsCount = newSeatsCount;
        AvailableSeats = newSeatsCount.Value - reservedSeats;
        return SetModificationData(DateTime.UtcNow);
    }

    public bool ChangeDescription(string? newDescription)
    {
        EnsureEditable();

        var normalizedDescription = newDescription?.Trim();

        if (Description == normalizedDescription)
            return false;

        Description = normalizedDescription;
        return SetModificationData(DateTime.UtcNow);
    }

    public bool Publish()
    {
        if (Status != TripStatus.Draft)
            throw new TripCannotBePublishedException(this);

        Status = TripStatus.Published;
        return SetModificationData(DateTime.UtcNow);
    }

    public bool Complete()
    {
        if (Status != TripStatus.Published)
            throw new InvalidOperationException("Only published trips can be completed");

        Status = TripStatus.Completed;
        return SetModificationData(DateTime.UtcNow);
    }

    public bool Cancel()
    {
        if (Status is TripStatus.Cancelled or TripStatus.Completed)
            throw new TripCannotBeCancelledException(this);

        Status = TripStatus.Cancelled;
        return SetModificationData(DateTime.UtcNow);
    }

    public bool BookSeats(SeatsCount seatsCount)
    {
        if (seatsCount == null)
            throw new ArgumentNullValueException(nameof(seatsCount));

        if (Status != TripStatus.Published)
            throw new TripCannotBeBookedException(this);

        if (AvailableSeats < seatsCount.Value)
            throw new TripHasNoAvailableSeatsException(this);

        AvailableSeats -= seatsCount.Value;
        return SetModificationData(DateTime.UtcNow);
    }

    public bool ReleaseSeats(SeatsCount seatsCount)
    {
        if (seatsCount == null)
            throw new ArgumentNullValueException(nameof(seatsCount));

        if (AvailableSeats + seatsCount.Value > SeatsCount.Value)
            throw new InvalidOperationException("Cannot release more seats than total seats");

        AvailableSeats += seatsCount.Value;
        return SetModificationData(DateTime.UtcNow);
    }

    public bool HasParticipant(User user)
    {
        if (user == null)
            return false;

        return Driver == user || _bookings.Any(x => x.Passenger == user && x.Status == BookingStatus.Approved);
    }

    public void AddBooking(Booking booking)
    {
        if (booking == null)
            throw new ArgumentNullValueException(nameof(booking));

        if (!_bookings.Contains(booking))
            _bookings.Add(booking);
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

    private void EnsureEditable()
    {
        if (Status is TripStatus.Cancelled or TripStatus.Completed)
            throw new TripCannotBeEditedException(this);
    }
}