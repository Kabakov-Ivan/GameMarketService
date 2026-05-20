using JointTripService.Domain.Entities;
using JointTripService.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JointTripService.Infrastructure.EntityFramework.Configurations;

public class BookingConfiguration : IEntityTypeConfiguration<Booking>
{
    public void Configure(EntityTypeBuilder<Booking> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).IsRequired();
        builder.Property(x => x.SeatsCount)
            .IsRequired()
            .HasConversion(seatsCount => seatsCount.Value, value => new SeatsCount(value));
        builder.Property(x => x.Status).IsRequired();
        builder.Property(x => x.CreationData).IsRequired().HasConversion
        (
            src => src.Kind == DateTimeKind.Utc ? src : DateTime.SpecifyKind(src, DateTimeKind.Utc),
            dst => dst.Kind == DateTimeKind.Utc ? dst : DateTime.SpecifyKind(dst, DateTimeKind.Utc)
        );
        builder.Property(x => x.ModificationData).IsRequired(false).HasConversion
        (
            src => !src.HasValue ? src : src.Value.Kind == DateTimeKind.Utc ? src : DateTime.SpecifyKind(src.Value, DateTimeKind.Utc),
            dst => !dst.HasValue ? dst : dst.Value.Kind == DateTimeKind.Utc ? dst : DateTime.SpecifyKind(dst.Value, DateTimeKind.Utc)
        );
        builder.HasOne(x => x.Passenger)
            .WithMany("_bookings")
            .HasForeignKey("PassengerId")
            .HasPrincipalKey(x => x.Id);
        builder.HasOne(x => x.Trip)
            .WithMany("_bookings")
            .HasForeignKey("TripId")
            .HasPrincipalKey(x => x.Id);
    }
}