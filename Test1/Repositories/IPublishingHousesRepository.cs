namespace Test1.Repositories;

public interface IPublishingHousesRepository
{
    Task<bool> DoesPublishingHouseExist(int publishingHouseId);
}