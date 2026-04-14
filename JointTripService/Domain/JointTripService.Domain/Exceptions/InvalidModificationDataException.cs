namespace JointTripService.Domain.Exceptions;

public class InvalidModificationDataException(object entity, DateTime modificationData)
    : ArgumentException($"The modification time {modificationData} is not correct")
{
    public object Entity => entity;
    public DateTime ModificationData => modificationData;
}