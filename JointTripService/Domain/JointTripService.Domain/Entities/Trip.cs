using JointTripService.Domain.Base;
using JointTripService.Domain.Exceptions;
using JointTripService.Domain.Enums;
using JointTripService.ValueObjects;

namespace JointTripService.Domain.Entities;

public class Trip : Entity<Guid>
{
    private readonly ICollection<Booking> _bookings = [];

    public Driver Driver { get; private set; } = default!;
    public Driver? PublishedByDriver { get; private set; }
    public Driver? CancelledByDriver { get; private set; }
    public Driver? CompletedByDriver { get; private set; }
    public City Origin { get; private set; } = default!;
    public City Destination { get; private set; } = default!;
    public DateTime DepartureAt { get; private set; }
    public SeatsCount SeatsCount { get; private set; } = default!;
    public int AvailableSeats { get; private set; }
    public TripDescription? Description { get; private set; }
    public TripStatus Status { get; private set; }
    public DateTime CreationData { get; }
    public DateTime? ModificationData { get; private set; }

    public IReadOnlyCollection<Booking> Bookings => _bookings.ToList().AsReadOnly();

    protected Trip()
    {
    }

    public Trip(Driver driver, City origin, City destination, DateTime departureAt, SeatsCount seatsCount, TripDescription? description = null, DateTime? creationData = null, DateTime? modificationData = null)
        : this(Guid.NewGuid(), driver, origin, destination, departureAt, seatsCount, description, creationData, modificationData)
    {
    }

    protected Trip(Guid id, Driver driver, City origin, City destination, DateTime departureAt, SeatsCount seatsCount, TripDescription? description = null, DateTime? creationData = null, DateTime? modificationData = null)
        : base(id)
    {
        if (id == Guid.Empty)
            throw new InvalidIdException();

        Driver = driver ?? throw new ArgumentNullValueException(nameof(driver));
        Origin = origin ?? throw new ArgumentNullValueException(nameof(origin));
        Destination = destination ?? throw new ArgumentNullValueException(nameof(destination));
        SeatsCount = seatsCount ?? throw new ArgumentNullValueException(nameof(seatsCount));

        if (Origin == Destination)
            throw new TripOriginAndDestinationMustBeDifferentException(this);

        DepartureAt = departureAt;
        AvailableSeats = seatsCount.Value;
        Description = description;
        Status = TripStatus.Draft;
        CreationData = creationData ?? DateTime.UtcNow;
        ModificationData = modificationData;
    }

    internal bool Edit(Driver editor, City newOrigin, City newDestination, DateTime newDepartureAt, SeatsCount newSeatsCount, TripDescription? newDescription = null)
    {
        if (editor == null)
            throw new ArgumentNullValueException(nameof(editor));

        if (Driver != editor)
            throw new AnotherDriverEditTripException(this, editor);

        if (_bookings.Any())
            throw new TripCannotBeEditedException(this);

        var isEdit = false;
        isEdit |= ChangeOrigin(newOrigin);
        isEdit |= ChangeDestination(newDestination);
        isEdit |= ChangeDepartureAt(newDepartureAt);
        isEdit |= ChangeSeatsCount(newSeatsCount);
        isEdit |= ChangeDescription(newDescription);

        return isEdit;
    }

    internal bool ChangeOrigin(City newOrigin)
    {
        EnsureEditable();

        if (newOrigin == null)
            throw new ArgumentNullValueException(nameof(newOrigin));

        if (Origin == newOrigin)
            return false;

        Origin = newOrigin;
        return SetModificationData(DateTime.UtcNow);
    }

    internal bool ChangeDestination(City newDestination)
    {
        EnsureEditable();

        if (newDestination == null)
            throw new ArgumentNullValueException(nameof(newDestination));

        if (Destination == newDestination)
            return false;

        Destination = newDestination;
        return SetModificationData(DateTime.UtcNow);
    }

    internal bool ChangeDepartureAt(DateTime newDepartureAt)
    {
        EnsureEditable();

        if (DepartureAt == newDepartureAt)
            return false;

        DepartureAt = newDepartureAt;
        return SetModificationData(DateTime.UtcNow);
    }

    internal bool ChangeSeatsCount(SeatsCount newSeatsCount)
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

    internal bool ChangeDescription(TripDescription? newDescription)
    {
        EnsureEditable();

        if (Description == newDescription)
            return false;

        Description = newDescription;
        return SetModificationData(DateTime.UtcNow);
    }

    internal bool Publish(Driver publisher)
    {
        if (publisher == null)
            throw new ArgumentNullValueException(nameof(publisher));

        if (Driver != publisher)
            throw new AnotherDriverEditTripException(this, publisher);

        if (Status != TripStatus.Draft)
            throw new TripCannotBePublishedException(this);

        PublishedByDriver = publisher;
        Status = TripStatus.Published;
        return SetModificationData(DateTime.UtcNow);
    }

    internal bool Complete(Driver completer)
    {
        if (completer == null)
            throw new ArgumentNullValueException(nameof(completer));

        if (Driver != completer)
            throw new AnotherDriverEditTripException(this, completer);

        if (Status != TripStatus.Published)
            throw new TripCannotBeCompletedInCurrentStateException(this);

        if (DateTime.UtcNow < DepartureAt)
            throw new TripCannotBeCompletedBeforeDepartureException(this, completer);

        CompletedByDriver = completer;
        Status = TripStatus.Completed;
        return SetModificationData(DateTime.UtcNow);
    }

    internal bool Cancel(Driver canceller)
    {
        if (canceller == null)
            throw new ArgumentNullValueException(nameof(canceller));

        if (Driver != canceller)
            throw new AnotherDriverEditTripException(this, canceller);

        if (Status is TripStatus.Cancelled or TripStatus.Completed)
            throw new TripCannotBeCancelledException(this);

        CancelledByDriver = canceller;
        Status = TripStatus.Cancelled;
        return SetModificationData(DateTime.UtcNow);
    }

    internal bool BookSeats(SeatsCount seatsCount)
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

    internal bool ReleaseSeats(SeatsCount seatsCount)
    {
        if (seatsCount == null)
            throw new ArgumentNullValueException(nameof(seatsCount));

        if (AvailableSeats + seatsCount.Value > SeatsCount.Value)
            throw new TripCannotReleaseTooManySeatsException(this);

        AvailableSeats += seatsCount.Value;
        return SetModificationData(DateTime.UtcNow);
    }

    public bool HasDriver(Driver driver)
    {
        if (driver == null)
            return false;

        return Driver.Id == driver.Id;
    }

    public bool HasPassenger(Passenger passenger)
    {
        if (passenger == null)
            return false;

        return _bookings.Any(x => x.Passenger.Id == passenger.Id && x.Status == BookingStatus.Approved);
    }

    internal void AddBooking(Booking booking)
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
        if (_bookings.Any())
            throw new TripCannotBeEditedException(this);

        if (Status is TripStatus.Cancelled or TripStatus.Completed)
            throw new TripCannotBeEditedException(this);
    }
}