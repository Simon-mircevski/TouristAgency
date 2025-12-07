using TouristAgencyAPI.Data;
using TouristAgencyAPI.Repositories;
using TouristAgencyShared.Models;

namespace TouristAgencyAPI.Repositories;

public class GuideRepository : IRepository<Guide>
{
    private readonly DatabaseContext _context;

    public GuideRepository(DatabaseContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Guide>> GetAllAsync()
    {
        const string sql = "SELECT * FROM guides ORDER BY CreatedAt DESC";
        return await _context.QueryAsync<Guide>(sql);
    }

    public async Task<Guide?> GetByIdAsync(int id)
    {
        const string sql = "SELECT * FROM guides WHERE Id = @Id";
        return await _context.QueryFirstOrDefaultAsync<Guide>(sql, new { Id = id });
    }

    public async Task<int> CreateAsync(Guide guide)
    {
        const string sql = @"
            INSERT INTO guides (FirstName, LastName, Email, Phone, Specialization, Languages, ExperienceYears, IsAvailable, CreatedAt, UpdatedAt)
            VALUES (@FirstName, @LastName, @Email, @Phone, @Specialization, @Languages, @ExperienceYears, @IsAvailable, @CreatedAt, @UpdatedAt);
            SELECT LAST_INSERT_ID();";

        guide.CreatedAt = DateTime.UtcNow;
        guide.UpdatedAt = DateTime.UtcNow;

        return await _context.ExecuteScalarAsync<int>(sql, guide);
    }

    public async Task<bool> UpdateAsync(Guide guide)
    {
        const string sql = @"
            UPDATE guides 
            SET FirstName = @FirstName, LastName = @LastName, Email = @Email, 
                Phone = @Phone, Specialization = @Specialization, Languages = @Languages, 
                ExperienceYears = @ExperienceYears, IsAvailable = @IsAvailable, 
                UpdatedAt = @UpdatedAt
            WHERE Id = @Id";

        guide.UpdatedAt = DateTime.UtcNow;

        var rowsAffected = await _context.ExecuteAsync(sql, guide);
        return rowsAffected > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        const string sql = "DELETE FROM guides WHERE Id = @Id";
        var rowsAffected = await _context.ExecuteAsync(sql, new { Id = id });
        return rowsAffected > 0;
    }
} 