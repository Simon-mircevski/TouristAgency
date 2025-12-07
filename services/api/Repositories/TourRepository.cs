using TouristAgencyAPI.Data;
using TouristAgencyAPI.Repositories;
using TouristAgencyShared.Models;
using Dapper;

namespace TouristAgencyAPI.Repositories;

public class TourRepository : IRepository<Tour>
{
    private readonly DatabaseContext _context;

    public TourRepository(DatabaseContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Tour>> GetAllAsync()
    {
        const string sql = @"
            SELECT t.*, d.*, g.*
            FROM tours t
            LEFT JOIN destinations d ON t.DestinationId = d.Id
            LEFT JOIN guides g ON t.GuideId = g.Id
            ORDER BY t.CreatedAt DESC";

        var tourDictionary = new Dictionary<int, Tour>();
        
        using var connection = _context.CreateConnection();
        await connection.QueryAsync<Tour, Destination, Guide, Tour>(
            sql,
            (tour, destination, guide) =>
            {
                if (!tourDictionary.TryGetValue(tour.Id, out var tourEntry))
                {
                    tourEntry = tour;
                    tourDictionary.Add(tour.Id, tourEntry);
                }
                
                tourEntry.Destination = destination;
                tourEntry.Guide = guide;
                
                return tourEntry;
            },
            splitOn: "Id");

        return tourDictionary.Values;
    }

    public async Task<Tour?> GetByIdAsync(int id)
    {
        const string sql = @"
            SELECT t.*, d.*, g.*
            FROM tours t
            LEFT JOIN destinations d ON t.DestinationId = d.Id
            LEFT JOIN guides g ON t.GuideId = g.Id
            WHERE t.Id = @Id";

        Tour? result = null;
        
        using var connection = _context.CreateConnection();
        await connection.QueryAsync<Tour, Destination, Guide, Tour>(
            sql,
            (tour, destination, guide) =>
            {
                if (result == null)
                {
                    result = tour;
                }
                
                result.Destination = destination;
                result.Guide = guide;
                
                return result;
            },
            new { Id = id },
            splitOn: "Id");

        return result;
    }

    public async Task<int> CreateAsync(Tour tour)
    {
        const string sql = @"
            INSERT INTO tours (Name, Description, DestinationId, GuideId, StartDate, EndDate, 
                              MaxParticipants, CurrentParticipants, Price, Status, CreatedAt, UpdatedAt)
            VALUES (@Name, @Description, @DestinationId, @GuideId, @StartDate, @EndDate, 
                    @MaxParticipants, @CurrentParticipants, @Price, @Status, @CreatedAt, @UpdatedAt);
            SELECT LAST_INSERT_ID();";

        tour.CreatedAt = DateTime.UtcNow;
        tour.UpdatedAt = DateTime.UtcNow;

        return await _context.ExecuteScalarAsync<int>(sql, tour);
    }

    public async Task<bool> UpdateAsync(Tour tour)
    {
        const string sql = @"
            UPDATE tours 
            SET Name = @Name, Description = @Description, DestinationId = @DestinationId, 
                GuideId = @GuideId, StartDate = @StartDate, EndDate = @EndDate, 
                MaxParticipants = @MaxParticipants, CurrentParticipants = @CurrentParticipants, 
                Price = @Price, Status = @Status, UpdatedAt = @UpdatedAt
            WHERE Id = @Id";

        tour.UpdatedAt = DateTime.UtcNow;

        var rowsAffected = await _context.ExecuteAsync(sql, tour);
        return rowsAffected > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        const string sql = "DELETE FROM tours WHERE Id = @Id";
        var rowsAffected = await _context.ExecuteAsync(sql, new { Id = id });
        return rowsAffected > 0;
    }
} 