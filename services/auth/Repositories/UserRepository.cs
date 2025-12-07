using Dapper;
using TouristAgencyAuth.Data;
using TouristAgencyShared.Models;

namespace TouristAgencyAuth.Repositories;

public class UserRepository
{
    private readonly DatabaseContext _context;

    public UserRepository(DatabaseContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<User>> GetAllAsync()
    {
        const string sql = @"
            SELECT Id, FirstName, LastName, Email, PasswordHash, Phone, Address, 
                   DateOfBirth, Role, IsActive, CreatedAt, UpdatedAt
            FROM users 
            WHERE IsActive = true
            ORDER BY CreatedAt DESC";

        return await _context.QueryAsync<User>(sql);
    }

    public async Task<User?> GetByIdAsync(int id)
    {
        const string sql = @"
            SELECT Id, FirstName, LastName, Email, PasswordHash, Phone, Address, 
                   DateOfBirth, Role, IsActive, CreatedAt, UpdatedAt
            FROM users 
            WHERE Id = @Id AND IsActive = true";

        return await _context.QueryFirstOrDefaultAsync<User>(sql, new { Id = id });
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        const string sql = @"
            SELECT Id, FirstName, LastName, Email, PasswordHash, Phone, Address, 
                   DateOfBirth, Role, IsActive, CreatedAt, UpdatedAt
            FROM users 
            WHERE Email = @Email AND IsActive = true";

        return await _context.QueryFirstOrDefaultAsync<User>(sql, new { Email = email });
    }

    public async Task<int> CreateAsync(User user)
    {
        const string sql = @"
            INSERT INTO users (FirstName, LastName, Email, PasswordHash, Phone, Address, 
                              DateOfBirth, Role, IsActive, CreatedAt, UpdatedAt)
            VALUES (@FirstName, @LastName, @Email, @PasswordHash, @Phone, @Address, 
                    @DateOfBirth, @Role, @IsActive, @CreatedAt, @UpdatedAt);
            SELECT LAST_INSERT_ID();";

        return await _context.ExecuteScalarAsync<int>(sql, user);
    }

    public async Task<bool> UpdateAsync(User user)
    {
        const string sql = @"
            UPDATE users 
            SET FirstName = @FirstName, LastName = @LastName, Email = @Email, 
                PasswordHash = @PasswordHash, Phone = @Phone, Address = @Address, 
                DateOfBirth = @DateOfBirth, Role = @Role, IsActive = @IsActive, 
                UpdatedAt = @UpdatedAt
            WHERE Id = @Id";

        var rowsAffected = await _context.ExecuteAsync(sql, user);
        return rowsAffected > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        const string sql = @"
            UPDATE users 
            SET IsActive = false, UpdatedAt = @UpdatedAt
            WHERE Id = @Id";

        var rowsAffected = await _context.ExecuteAsync(sql, new { Id = id, UpdatedAt = DateTime.UtcNow });
        return rowsAffected > 0;
    }

    public async Task<bool> EmailExistsAsync(string email)
    {
        const string sql = "SELECT COUNT(1) FROM users WHERE Email = @Email";
        var count = await _context.ExecuteScalarAsync<int>(sql, new { Email = email });
        return count > 0;
    }
}