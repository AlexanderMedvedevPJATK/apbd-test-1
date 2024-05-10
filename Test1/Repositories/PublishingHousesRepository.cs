using Microsoft.Data.SqlClient;

namespace Test1.Repositories;

public class PublishingHousesRepository : IPublishingHousesRepository
{

    private readonly IConfiguration _configuration;
    public PublishingHousesRepository(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    public async Task<bool> DoesPublishingHouseExist(int id)
    {
        var query = "SELECT 1 FROM publishing_houses WHERE PK = @ID";

        await using var connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        await using var command = new SqlCommand();

        command.Connection = connection;
        command.CommandText = query;
        command.Parameters.AddWithValue("@ID", id);

        await connection.OpenAsync();

        var res = await command.ExecuteScalarAsync();

        return res is not null;
    }
}