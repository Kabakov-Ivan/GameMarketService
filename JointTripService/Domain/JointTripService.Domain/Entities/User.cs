using JointTripService.Domain.Base;
using JointTripService.Domain.Exceptions;
using JointTripService.ValueObjects;

namespace JointTripService.Domain.Entities;

public class User : Entity<Guid>
{
    private readonly ICollection<Trip> _trips = [];
    private readonly ICollection<Booking> _bookings = [];
    private readonly ICollection<Review> _reviewsWritten = [];
    private readonly ICollection<Review> _reviewsReceived = [];

    public FullName FullName { get; private set; } = default!;
    public Email Email { get; private set; } = default!;
    public DateTime CreationData { get; }
    public DateTime? ModificationData { get; private set; }

    public IReadOnlyCollection<Trip> Trips => _trips.ToList().AsReadOnly();
    public IReadOnlyCollection<Booking> Bookings => _bookings.ToList().AsReadOnly();
    public IReadOnlyCollection<Review> ReviewsWritten => _reviewsWritten.ToList().AsReadOnly();
    public IReadOnlyCollection<Review> ReviewsReceived => _reviewsReceived.ToList().AsReadOnly();

    protected User()
    {
    }

    public User(Guid id, FullName fullName, Email email) : base(id)
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

    public Trip CreateTrip(City origin, City destination, DateTime departureAt, int seatsCount, string? description = null)
    {
        var trip = new Trip(this, origin, destination, departureAt, seatsCount, description);
        _trips.Add(trip);
        return trip;
    }

    public bool EditTrip(Trip trip, City origin, City destination, DateTime departureAt, int seatsCount, string? description = null)
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

    public Booking CreateBooking(Trip trip, int seatsCount)
    {
        if (trip == null)
            throw new ArgumentNullValueException(nameof(trip));

        if (trip.Driver == this)
            throw new TripCannotBeBookedByDriverException(trip);

        var booking = new Booking(this, trip, seatsCount);
        _bookings.Add(booking);
        return booking;
    }

    public Review LeaveReview(User targetUser, Trip trip, int rating, ReviewText text)
    {
        if (targetUser == null)
            throw new ArgumentNullValueException(nameof(targetUser));

        if (trip == null)
            throw new ArgumentNullValueException(nameof(trip));

        if (text == null)
            throw new ArgumentNullValueException(nameof(text));

        if (targetUser == this)
            throw new UserCannotReviewHimselfException(this);

        if (!trip.HasParticipant(this) || !trip.HasParticipant(targetUser))
            throw new InvalidOperationException("Users must be trip participants");

        var review = new Review(this, targetUser, trip, rating, text);
        _reviewsWritten.Add(review);
        targetUser._reviewsReceived.Add(review);
        return review;
    }
}