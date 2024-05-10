using Microsoft.Data.SqlClient;
using Test1.DTOs;

namespace Test1.Repositories;

public class BookRepository : IBookRepository
{
    
    private readonly IConfiguration _configuration;
    public BookRepository(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    public async Task<bool> DoesBookExist(int id)
    {
        var query = "SELECT 1 FROM books WHERE PK = @ID";

        await using var connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        await using var command = new SqlCommand();

        command.Connection = connection;
        command.CommandText = query;
        command.Parameters.AddWithValue("@ID", id);

        await connection.OpenAsync();

        var res = await command.ExecuteScalarAsync();

        return res is not null;
    }

    public async Task<bool> DoesBookHaveEditions(int id)
    {
        var query = "SELECT 1 FROM books_editions WHERE FK_book = @ID";

        await using var connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        await using var command = new SqlCommand();

        command.Connection = connection;
        command.CommandText = query;
        command.Parameters.AddWithValue("@ID", id);

        await connection.OpenAsync();

        var res = await command.ExecuteScalarAsync();

        return res is not null;
    }

    public async Task<IEnumerable<BookEditionsDto>> GetBookEditions(int id)
    {
        var query = @"SELECT 
                            books_editions.PK AS editionID,
                            title,
                            edition_title,
                            name,    
                            release_date
                      FROM books
                      JOIN books_editions ON books.PK = books_editions.FK_book
                      JOIN publishing_houses ON books_editions.FK_publishing_house = publishing_houses.PK
                      WHERE books.PK = @ID";
        
        await using var connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        await using var command = new SqlCommand();

        command.Connection = connection;
        command.CommandText = query;
        command.Parameters.AddWithValue("@ID", id);

        await connection.OpenAsync();

        var reader = await command.ExecuteReaderAsync();

        var editionIdOrdinal = reader.GetOrdinal("editionID");
        var titleOrdinal = reader.GetOrdinal("title");
        var editionTitleOrdinal = reader.GetOrdinal("edition_title");
        var nameOrdinal = reader.GetOrdinal("name");
        var releaseDateOrdinal = reader.GetOrdinal("release_date");
        
        var bookEditionsDtos = new List<BookEditionsDto>();
        
        while (await reader.ReadAsync())
        {
            bookEditionsDtos.Add(new BookEditionsDto()
            {
                Id = reader.GetInt32(editionIdOrdinal),
                BookTitle = reader.GetString(titleOrdinal),
                EditionTitle = reader.GetString(editionTitleOrdinal),
                PublishingHouseName = reader.GetString(nameOrdinal),
                ReleaseDate = reader.GetDateTime(releaseDateOrdinal)
            });
        }
        
        if (bookEditionsDtos.Count == 0) throw new Exception();

        return bookEditionsDtos;
    }

    public async Task AddBookWithPublishingHouse(AddBookWithPublisherDto addBookWithPublisherDto)
    {
        var insert = @"INSERT INTO books VALUES(@Title);
					   SELECT @@IDENTITY AS ID;";
	    
        await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        await using SqlCommand command = new SqlCommand();
	    
        command.Connection = connection;
        command.CommandText = insert;

        command.Parameters.AddWithValue("@Title", addBookWithPublisherDto.BookTitle);
        	    
        await connection.OpenAsync();

        var transaction = await connection.BeginTransactionAsync();
        command.Transaction = transaction as SqlTransaction;
	    
        try
        {
            var id = await command.ExecuteScalarAsync();
            
            command.Parameters.Clear();
            command.CommandText = "INSERT INTO books_editions VALUES(@PublisherId, @BookId, @Title, @ReleaseDate)";
            command.Parameters.AddWithValue("@PublisherId", addBookWithPublisherDto.PublishingHouseId);
            command.Parameters.AddWithValue("@BookId", id);
            command.Parameters.AddWithValue("@Title", addBookWithPublisherDto.EditionTitle);
            command.Parameters.AddWithValue("@ReleaseDate", addBookWithPublisherDto.ReleaseDate);

            await command.ExecuteNonQueryAsync();

            await transaction.CommitAsync();
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}