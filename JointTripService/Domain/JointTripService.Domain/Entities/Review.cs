using JointTripService.Domain.Base;
using JointTripService.Domain.Exceptions;
using JointTripService.ValueObjects;

namespace JointTripService.Domain.Entities;

public class Review : Entity<Guid>
{
    public Guid AuthorId { get; private set; }
    public Guid TargetId { get; private set; }
    public bool AuthorIsDriver { get; private set; }
    public bool TargetIsDriver { get; private set; }
    public Driver? AuthorDriver { get; private set; }
    public Passenger? AuthorPassenger { get; private set; }
    public Driver? TargetDriver { get; private set; }
    public Passenger? TargetPassenger { get; private set; }
    public Trip Trip { get; private set; } = default!;
    public ReviewRating Rating { get; private set; } = default!;
    public ReviewText Text { get; private set; } = default!;
    public DateTime CreationData { get; }
    public DateTime? ModificationData { get; private set; }

    protected Review()
    {
    }

    public Review(Driver author, Passenger targetPassenger, Trip trip, ReviewRating rating, ReviewText text, DateTime? creationData = null, DateTime? modificationData = null)
        : this(Guid.NewGuid(), author, targetPassenger, trip, rating, text, creationData, modificationData)
    {
    }

    public Review(Passenger author, Driver targetDriver, Trip trip, ReviewRating rating, ReviewText text, DateTime? creationData = null, DateTime? modificationData = null)
        : this(Guid.NewGuid(), author, targetDriver, trip, rating, text, creationData, modificationData)
    {
    }

    protected Review(Guid id, Driver author, Passenger targetPassenger, Trip trip, ReviewRating rating, ReviewText text, DateTime? creationData = null, DateTime? modificationData = null)
        : base(id)
    {
        if (id == Guid.Empty)
            throw new InvalidIdException();

        AuthorId = author?.Id ?? throw new ArgumentNullValueException(nameof(author));
        TargetId = targetPassenger?.Id ?? throw new ArgumentNullValueException(nameof(targetPassenger));
        AuthorIsDriver = true;
        TargetIsDriver = false;
        AuthorDriver = author;
        TargetPassenger = targetPassenger;
        Trip = trip ?? throw new ArgumentNullValueException(nameof(trip));
        Rating = rating ?? throw new ArgumentNullValueException(nameof(rating));
        Text = text ?? throw new ArgumentNullValueException(nameof(text));

        if (author.Id == targetPassenger.Id)
            throw new DriverCannotReviewHimselfException(author);

        if (!Trip.HasDriver(author) || !Trip.HasPassenger(targetPassenger))
            throw new TripParticipantsMustBelongToSameTripException(trip);

        CreationData = creationData ?? DateTime.UtcNow;
        ModificationData = modificationData;
    }

    protected Review(Guid id, Passenger author, Driver targetDriver, Trip trip, ReviewRating rating, ReviewText text, DateTime? creationData = null, DateTime? modificationData = null)
        : base(id)
    {
        if (id == Guid.Empty)
            throw new InvalidIdException();

        AuthorId = author?.Id ?? throw new ArgumentNullValueException(nameof(author));
        TargetId = targetDriver?.Id ?? throw new ArgumentNullValueException(nameof(targetDriver));
        AuthorIsDriver = false;
        TargetIsDriver = true;
        AuthorPassenger = author;
        TargetDriver = targetDriver;
        Trip = trip ?? throw new ArgumentNullValueException(nameof(trip));
        Rating = rating ?? throw new ArgumentNullValueException(nameof(rating));
        Text = text ?? throw new ArgumentNullValueException(nameof(text));

        if (author.Id == targetDriver.Id)
            throw new PassengerCannotReviewHimselfException(author);

        if (!Trip.HasPassenger(author) || !Trip.HasDriver(targetDriver))
            throw new TripParticipantsMustBelongToSameTripException(trip);

        CreationData = creationData ?? DateTime.UtcNow;
        ModificationData = modificationData;
    }

    public bool ChangeRating(ReviewRating rating)
    {
        if (rating == null)
            throw new ArgumentNullValueException(nameof(rating));

        if (Rating == rating)
            return false;

        Rating = rating;
        return SetModificationData(DateTime.UtcNow);
    }

    public bool ChangeText(ReviewText newText)
    {
        if (newText == null)
            throw new ArgumentNullValueException(nameof(newText));

        if (Text == newText)
            return false;

        Text = newText;
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