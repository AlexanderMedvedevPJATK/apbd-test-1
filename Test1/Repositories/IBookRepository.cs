using Test1.DTOs;

namespace Test1.Repositories;

public interface IBookRepository
{
    Task<bool> DoesBookExist(int id);

    Task<bool> DoesBookHaveEditions(int id);
    
    Task<IEnumerable<BookEditionsDto>> GetBookEditions(int id);

    Task AddBookWithPublishingHouse(AddBookWithPublisherDto addBookWithPublisherDto);
}