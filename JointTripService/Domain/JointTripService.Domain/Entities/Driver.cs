using JointTripService.Domain.Base;
using JointTripService.Domain.Enums;
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
    public IReadOnlyCollection<Trip> UpcomingTrips => _trips
        .Where(trip => trip.Status == TripStatus.Published && trip.DepartureAt > DateTime.UtcNow)
        .ToList()
        .AsReadOnly();
    public IReadOnlyCollection<Booking> RequestedBookings => _trips
        .SelectMany(trip => trip.Bookings)
        .Where(booking => booking.Status == BookingStatus.Pending)
        .ToList()
        .AsReadOnly();
    public IReadOnlyCollection<Review> ReviewsWritten => _reviewsWritten.ToList().AsReadOnly();
    public IReadOnlyCollection<Review> ReviewsReceived => _reviewsReceived.ToList().AsReadOnly();

    protected Driver()
    {
    }

    public Driver(Guid id, FullName fullName, Email email, DateTime? creationData = null) : base(id)
    {
        if (id == Guid.Empty)
            throw new InvalidIdException();

        FullName = fullName ?? throw new ArgumentNullValueException(nameof(fullName));
        Email = email ?? throw new ArgumentNullValueException(nameof(email));
        CreationData = creationData ?? DateTime.UtcNow;
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

    public Trip CreateTrip(City origin, City destination, DateTime departureAt, SeatsCount seatsCount, TripDescription? description = null)
    {
        var trip = new Trip(this, origin, destination, departureAt, seatsCount, description);
        _trips.Add(trip);
        return trip;
    }

    public bool PublishTrip(Trip trip)
    {
        if (trip == null)
            throw new ArgumentNullValueException(nameof(trip));

        if (trip.Driver != this)
            throw new AnotherDriverEditTripException(trip, this);

        var isPublished = trip.Publish(this);

        return isPublished;
    }

    public bool EditTrip(Trip trip, City origin, City destination, DateTime departureAt, SeatsCount seatsCount, TripDescription? description = null)
    {
        if (trip == null)
            throw new ArgumentNullValueException(nameof(trip));

        if (trip.Driver != this)
            throw new AnotherDriverEditTripException(trip, this);

        return trip.Edit(this, origin, destination, departureAt, seatsCount, description);
    }

    public bool CancelTrip(Trip trip)
    {
        if (trip == null)
            throw new ArgumentNullValueException(nameof(trip));

        if (trip.Driver != this)
            throw new AnotherDriverEditTripException(trip, this);

        var isCancel = trip.Cancel(this);

        return isCancel;
    }

    public bool CompleteTrip(Trip trip)
    {
        if (trip == null)
            throw new ArgumentNullValueException(nameof(trip));

        if (trip.Driver != this)
            throw new AnotherDriverEditTripException(trip, this);

        var isCompleted = trip.Complete(this);

        return isCompleted;
    }

    public Review LeaveReview(Passenger targetPassenger, Trip trip, ReviewRating rating, ReviewText text)
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
            throw new TripParticipantsMustBelongToSameTripException(trip);

        var review = new Review(this, targetPassenger, trip, rating, text);
        _reviewsWritten.Add(review);
        targetPassenger.AddReceivedReview(review);
        _reviewsReceived.Add(review);
        return review;
    }

    internal void AddReceivedReview(Review review)
    {
        if (review == null)
            throw new ArgumentNullValueException(nameof(review));

        if (review.TargetDriver != this)
            throw new ReviewIsNotForThisParticipantException(review, FullName.ToString());

        if (review.AuthorPassenger == null)
            throw new ReviewAuthorDidNotParticipateInTripException(review, "неизвестный пассажир");

        if (!review.Trip.HasPassenger(review.AuthorPassenger))
            throw new ReviewAuthorDidNotParticipateInTripException(review, review.AuthorPassenger.FullName.ToString());

        if (!_reviewsReceived.Contains(review))
            _reviewsReceived.Add(review);
    }

    public bool ConfirmBooking(Booking booking)
    {
        if (booking == null)
            throw new ArgumentNullValueException(nameof(booking));

        if (booking.Trip.Driver != this)
            throw new DriverCannotManageAnotherDriversBookingException(this, booking);

        return booking.Confirm(this);
    }

    public bool ApproveBooking(Booking booking)
        => ConfirmBooking(booking);

    public bool RejectBooking(Booking booking)
    {
        if (booking == null)
            throw new ArgumentNullValueException(nameof(booking));

        if (booking.Trip.Driver != this)
            throw new DriverCannotManageAnotherDriversBookingException(this, booking);

        return booking.RejectBooking(this);
    }
}