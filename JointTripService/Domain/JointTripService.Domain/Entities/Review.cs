using JointTripService.Domain.Base;
using JointTripService.Domain.Exceptions;
using JointTripService.ValueObjects;

namespace JointTripService.Domain.Entities;

public class Review : Entity<Guid>
{
    public User Author { get; private set; } = default!;
    public User TargetUser { get; private set; } = default!;
    public Trip Trip { get; private set; } = default!;
    public int Rating { get; private set; }
    public ReviewText Text { get; private set; } = default!;
    public DateTime CreationData { get; }
    public DateTime? ModificationData { get; private set; }

    protected Review()
    {
    }

    public Review(User author, User targetUser, Trip trip, int rating, ReviewText text)
        : this(Guid.NewGuid(), author, targetUser, trip, rating, text)
    {
    }

    protected Review(Guid id, User author, User targetUser, Trip trip, int rating, ReviewText text)
        : base(id)
    {
        if (id == Guid.Empty)
            throw new InvalidIdException();

        Author = author ?? throw new ArgumentNullValueException(nameof(author));
        TargetUser = targetUser ?? throw new ArgumentNullValueException(nameof(targetUser));
        Trip = trip ?? throw new ArgumentNullValueException(nameof(trip));
        Text = text ?? throw new ArgumentNullValueException(nameof(text));

        if (Author == TargetUser)
            throw new UserCannotReviewHimselfException(author);

        if (!Trip.HasParticipant(Author) || !Trip.HasParticipant(TargetUser))
            throw new InvalidOperationException("Users must be trip participants");

        if (rating is < 1 or > 5)
            throw new InvalidRatingException(rating);

        Rating = rating;
        CreationData = DateTime.UtcNow;
    }

    public bool ChangeRating(int rating)
    {
        if (rating is < 1 or > 5)
            throw new InvalidRatingException(rating);

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