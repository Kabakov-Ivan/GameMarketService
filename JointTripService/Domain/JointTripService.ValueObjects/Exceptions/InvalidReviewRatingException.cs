namespace JointTripService.ValueObjects.Exceptions;

public class InvalidReviewRatingException(int rating)
    : ArgumentOutOfRangeException(nameof(rating), rating, "Оценка отзыва должна быть в диапазоне от 1 до 5");