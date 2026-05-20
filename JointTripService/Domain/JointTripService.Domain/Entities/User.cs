using JointTripService.Domain.Base;
using JointTripService.Domain.Exceptions;
using JointTripService.ValueObjects;

namespace JointTripService.Domain.Entities;

public abstract class User : Entity<Guid>
{
    private readonly ICollection<Review> _reviewsWritten = [];
    private readonly ICollection<Review> _reviewsReceived = [];

    public FullName FullName { get; private set; } = default!;
    public Email Email { get; private set; } = default!;
    public DateTime CreationData { get; }
    public DateTime? ModificationData { get; protected set; }

    public IReadOnlyCollection<Review> ReviewsWritten => _reviewsWritten.ToList().AsReadOnly();
    public IReadOnlyCollection<Review> ReviewsReceived => _reviewsReceived.ToList().AsReadOnly();

    protected User()
    {
    }

    protected User(Guid id, FullName fullName, Email email) : base(id)
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
        ModificationData = DateTime.UtcNow;
        return review;
    }
}