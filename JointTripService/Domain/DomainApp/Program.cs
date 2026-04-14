using JointTripService.Domain.Entities;
using JointTripService.Domain.Enums;
using JointTripService.ValueObjects;

var driver = new User(Guid.NewGuid(), new FullName("Ivan Petrov"), new Email("ivan.petrov@example.com"));
var passenger = new User(Guid.NewGuid(), new FullName("Anna Smirnova"), new Email("anna.smirnova@example.com"));

var trip = driver.CreateTrip(
    new City("Москва"),
    new City("Тула"),
    DateTime.UtcNow.AddDays(1),
    3,
    "Утренняя поездка");

trip.Publish();

var booking = passenger.CreateBooking(trip, 1);
booking.Approve();

var review = passenger.LeaveReview(driver, trip, 5, new ReviewText("Хорошая поездка"));

Console.WriteLine($"Поездка: {trip.Origin} -> {trip.Destination}");
Console.WriteLine($"Свободных мест: {trip.AvailableSeats}");
Console.WriteLine($"Статус бронирования: {booking.Status switch
{
    BookingStatus.Pending => "В ожидании",
    BookingStatus.Approved => "Подтверждено",
    BookingStatus.Rejected => "Отклонено",
    BookingStatus.Cancelled => "Отменено",
    _ => booking.Status.ToString()
}}");
Console.WriteLine($"Оценка отзыва: {review.Rating}");