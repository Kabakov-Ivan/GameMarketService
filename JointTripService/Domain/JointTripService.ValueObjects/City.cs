using JointTripService.ValueObjects.Base;
using JointTripService.ValueObjects.Validators;

namespace JointTripService.ValueObjects;

public class City(string value) : ValueObject<string>(new CityValidator(), value.Trim());