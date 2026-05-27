using JointTripService.Domain.Base;
using JointTripService.Domain.Exceptions;
using JointTripService.ValueObjects;

namespace JointTripService.Domain.Entities;

public class Passenger : Entity<Guid>
{
    private readonly ICollection<Booking> _bookings = [];
    private readonly ICollection<Review> _reviewsWritten = [];
    private readonly ICollection<Review> _reviewsReceived = [];

    public FullName FullName { get; private set; } = default!;
    public Email Email { get; private set; } = default!;
    public DateTime CreationData { get; }
    public DateTime? ModificationData { get; protected set; }

    public IReadOnlyCollection<Booking> Bookings => _bookings.ToList().AsReadOnly();
    public IReadOnlyCollection<Review> ReviewsWritten => _reviewsWritten.ToList().AsReadOnly();
    public IReadOnlyCollection<Review> ReviewsReceived => _reviewsReceived.ToList().AsReadOnly();

    protected Passenger()
    {
    }

    public Passenger(Guid id, FullName fullName, Email email, DateTime? creationData = null, DateTime? modificationData = null) : base(id)
    {
        if (id == Guid.Empty)
            throw new InvalidIdException();

        FullName = fullName ?? throw new ArgumentNullValueException(nameof(fullName));
        Email = email ?? throw new ArgumentNullValueException(nameof(email));
        CreationData = creationData ?? DateTime.UtcNow;
        ModificationData = modificationData;
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

    public Review LeaveReview(Driver targetDriver, Trip trip, ReviewRating rating, ReviewText text)
    {
        if (targetDriver == null)
            throw new ArgumentNullValueException(nameof(targetDriver));

        if (trip == null)
            throw new ArgumentNullValueException(nameof(trip));

        if (text == null)
            throw new ArgumentNullValueException(nameof(text));

        if (targetDriver.Id == Id)
            throw new PassengerCannotReviewHimselfException(this);

        if (!trip.HasPassenger(this) || !trip.HasDriver(targetDriver))
            throw new TripParticipantsMustBelongToSameTripException(trip);

        var review = new Review(this, targetDriver, trip, rating, text);
        _reviewsWritten.Add(review);
        targetDriver.AddReceivedReview(review);
        _reviewsReceived.Add(review);
        return review;
    }

    internal void AddReceivedReview(Review review)
    {
        if (review == null)
            throw new ArgumentNullValueException(nameof(review));

        if (review.TargetPassenger != this)
            throw new ReviewIsNotForThisParticipantException(review, FullName.ToString());

        if (review.AuthorDriver == null)
            throw new ReviewAuthorDidNotParticipateInTripException(review, "неизвестный водитель");

        if (!review.Trip.HasDriver(review.AuthorDriver))
            throw new ReviewAuthorDidNotParticipateInTripException(review, review.AuthorDriver.FullName.ToString());

        if (!_reviewsReceived.Contains(review))
            _reviewsReceived.Add(review);
    }

    public Booking BookTrip(Trip trip, SeatsCount seatsCount)
    {
        if (trip == null)
            throw new ArgumentNullValueException(nameof(trip));

        if (trip.Driver.Email == Email)
            throw new TripCannotBeBookedByDriverException(trip);

        var booking = new Booking(this, trip, seatsCount);
        _bookings.Add(booking);
        trip.AddBooking(booking);
        return booking;
    }
}