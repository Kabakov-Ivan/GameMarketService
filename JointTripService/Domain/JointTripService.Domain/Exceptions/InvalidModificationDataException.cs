namespace JointTripService.Domain.Exceptions;

public class InvalidModificationDataException(object entity, DateTime modificationData)
    : ArgumentException($"Время изменения {modificationData} указано некорректно")
{
    public object Entity => entity;
    public DateTime ModificationData => modificationData;
}