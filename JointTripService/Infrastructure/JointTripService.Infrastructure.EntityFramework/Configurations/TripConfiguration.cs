using JointTripService.Domain.Entities;
using JointTripService.ValueObjects;
using JointTripService.ValueObjects.Validators;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JointTripService.Infrastructure.EntityFramework.Configurations;

public class TripConfiguration : IEntityTypeConfiguration<Trip>
{
    public void Configure(EntityTypeBuilder<Trip> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).IsRequired();
        builder.Property(x => x.Origin)
            .IsRequired()
            .HasConversion(origin => origin.Value, value => new City(value))
            .HasMaxLength(CityValidator.MAX_LENGTH);
        builder.Property(x => x.Destination)
            .IsRequired()
            .HasConversion(destination => destination.Value, value => new City(value))
            .HasMaxLength(CityValidator.MAX_LENGTH);
        builder.Property(x => x.DepartureAt).IsRequired();
        builder.Property(x => x.SeatsCount)
            .IsRequired()
            .HasConversion(seatsCount => seatsCount.Value, value => new SeatsCount(value));
        builder.Property(x => x.AvailableSeats).IsRequired();
        builder.Property(x => x.Description).IsRequired(false);
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
        builder.HasOne(x => x.Driver)
            .WithMany("_trips")
            .HasForeignKey("DriverId")
            .HasPrincipalKey(x => x.Id);
        builder.Ignore(x => x.Bookings);
    }
}
