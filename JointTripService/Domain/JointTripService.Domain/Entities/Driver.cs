using JointTripService.Domain.Base;
using JointTripService.Domain.Exceptions;
using JointTripService.ValueObjects;

namespace JointTripService.Domain.Entities;

public class Driver : Entity<Guid>
{
    private readonly ICollection<Trip> _trips = [];
    private readonly ICollection<Review> _reviewsWritten = [];
    private readonly ICollection<Review> _reviewsReceived = [];

    public FullName FullName { get; private set; } = default!;
    public Email Email { get; private set; } = default!;
    public DateTime CreationData { get; }
    public DateTime? ModificationData { get; protected set; }

    public IReadOnlyCollection<Trip> Trips => _trips.ToList().AsReadOnly();
    public IReadOnlyCollection<Review> ReviewsWritten => _reviewsWritten.ToList().AsReadOnly();
    public IReadOnlyCollection<Review> ReviewsReceived => _reviewsReceived.ToList().AsReadOnly();

    protected Driver()
    {
    }

    public Driver(Guid id, FullName fullName, Email email) : base(id)
    {
        if (id == Guid.Empty)
            throw new InvalidIdException();

        FullName = fullName ?? throw new ArgumentNullValueException(nameof(fullName));
        Email = email ?? throw new ArgumentNullValueException(nameof(email));
        CreationData = DateTime.UtcNow;
    }

    public bool ChangeFullName(FullName newFullName)
    {
        if (newFullName == null)
            throw new ArgumentNullValueException(nameof(newFullName));

        if (FullName == newFullName)
            return false;

        FullName = newFullName;
        ModificationData = DateTime.UtcNow;
        return true;
    }

    public bool ChangeEmail(Email newEmail)
    {
        if (newEmail == null)
            throw new ArgumentNullValueException(nameof(newEmail));

        if (Email == newEmail)
            return false;

        Email = newEmail;
        ModificationData = DateTime.UtcNow;
        return true;
    }

    public Trip CreateTrip(City origin, City destination, DateTime departureAt, SeatsCount seatsCount, string? description = null)
    {
        var trip = new Trip(this, origin, destination, departureAt, seatsCount, description);
        _trips.Add(trip);
        ModificationData = DateTime.UtcNow;
        return trip;
    }

    public bool PublishTrip(Trip trip)
    {
        if (trip == null)
            throw new ArgumentNullValueException(nameof(trip));

        if (trip.Driver != this)
            throw new AnotherDriverEditTripException(trip, this);

        if (!_trips.Contains(trip))
            throw new AnotherDriverEditTripException(trip, this);

        var isPublished = trip.Publish();

        if (isPublished)
            ModificationData = DateTime.UtcNow;

        return isPublished;
    }

    public bool EditTrip(Trip trip, City origin, City destination, DateTime departureAt, SeatsCount seatsCount, string? description = null)
    {
        if (trip == null)
            throw new ArgumentNullValueException(nameof(trip));

        if (trip.Driver != this)
            throw new AnotherDriverEditTripException(trip, this);

        if (!_trips.Contains(trip))
            throw new AnotherDriverEditTripException(trip, this);

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
            throw new AnotherDriverEditTripException(trip, this);

        if (!_trips.Contains(trip))
            throw new AnotherDriverEditTripException(trip, this);

        var isCancel = trip.Cancel();

        if (isCancel)
            ModificationData = DateTime.UtcNow;

        return isCancel;
    }

    public bool CompleteTrip(Trip trip)
    {
        if (trip == null)
            throw new ArgumentNullValueException(nameof(trip));

        if (trip.Driver != this)
            throw new AnotherDriverEditTripException(trip, this);

        if (!_trips.Contains(trip))
            throw new AnotherDriverEditTripException(trip, this);

        var isCompleted = trip.Complete();

        if (isCompleted)
            ModificationData = DateTime.UtcNow;

        return isCompleted;
    }

    public Review LeaveReview(Passenger targetPassenger, Trip trip, int rating, ReviewText text)
    {
        if (targetPassenger == null)
            throw new ArgumentNullValueException(nameof(targetPassenger));

        if (trip == null)
            throw new ArgumentNullValueException(nameof(trip));

        if (text == null)
            throw new ArgumentNullValueException(nameof(text));

        if (targetPassenger.Id == Id)
            throw new DriverCannotReviewHimselfException(this);

        if (!trip.HasDriver(this) || !trip.HasPassenger(targetPassenger))
            throw new InvalidOperationException("Водитель и пассажир должны относиться к одной поездке");

        var review = new Review(this, targetPassenger, trip, rating, text);
        _reviewsWritten.Add(review);
        targetPassenger.AddReceivedReview(review);
        _reviewsReceived.Add(review);
        ModificationData = DateTime.UtcNow;
        return review;
    }

    internal void AddReceivedReview(Review review)
    {
        if (review == null)
            throw new ArgumentNullValueException(nameof(review));

        if (!_reviewsReceived.Contains(review))
            _reviewsReceived.Add(review);
    }

    public bool ConfirmBooking(Booking booking)
    {
        if (booking == null)
            throw new ArgumentNullValueException(nameof(booking));

        if (booking.Trip.Driver != this)
            throw new InvalidOperationException("Водитель не может подтверждать бронирование чужой поездки");

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
            throw new InvalidOperationException("Водитель не может отклонять бронирование чужой поездки");

        var isRejected = booking.RejectBooking();

        if (isRejected)
            ModificationData = DateTime.UtcNow;

        return isRejected;
    }
}