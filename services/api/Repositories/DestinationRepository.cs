using TouristAgencyAPI.Data;
using TouristAgencyAPI.Repositories;
using TouristAgencyShared.Models;

namespace TouristAgencyAPI.Repositories;

public class DestinationRepository : IRepository<Destination>
{
    private readonly DatabaseContext _context;

    public DestinationRepository(DatabaseContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Destination>> GetAllAsync()
    {
        const string sql = "SELECT * FROM destinations ORDER BY CreatedAt DESC";
        return await _context.QueryAsync<Destination>(sql);
    }

    public async Task<Destination?> GetByIdAsync(int id)
    {
        const string sql = "SELECT * FROM destinations WHERE Id = @Id";
        return await _context.QueryFirstOrDefaultAsync<Destination>(sql, new { Id = id });
    }

    public async Task<int> CreateAsync(Destination destination)
    {
        const string sql = @"
            INSERT INTO destinations (Name, Description, Country, City, ImageUrl, Price, Duration, CreatedAt, UpdatedAt)
            VALUES (@Name, @Description, @Country, @City, @ImageUrl, @Price, @Duration, @CreatedAt, @UpdatedAt);
            SELECT LAST_INSERT_ID();";

        destination.CreatedAt = DateTime.UtcNow;
        destination.UpdatedAt = DateTime.UtcNow;

        return await _context.ExecuteScalarAsync<int>(sql, destination);
    }

    public async Task<bool> UpdateAsync(Destination destination)
    {
        const string sql = @"
            UPDATE destinations 
            SET Name = @Name, Description = @Description, Country = @Country, 
                City = @City, ImageUrl = @ImageUrl, Price = @Price, Duration = @Duration, 
                UpdatedAt = @UpdatedAt
            WHERE Id = @Id";

        destination.UpdatedAt = DateTime.UtcNow;

        var rowsAffected = await _context.ExecuteAsync(sql, destination);
        return rowsAffected > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        const string sql = "DELETE FROM destinations WHERE Id = @Id";
        var rowsAffected = await _context.ExecuteAsync(sql, new { Id = id });
        return rowsAffected > 0;
    }
} 