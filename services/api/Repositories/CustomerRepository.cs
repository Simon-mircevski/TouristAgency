using TouristAgencyAPI.Data;
using TouristAgencyAPI.Repositories;
using TouristAgencyShared.Models;

namespace TouristAgencyAPI.Repositories;

public class CustomerRepository : IRepository<Customer>
{
    private readonly DatabaseContext _context;

    public CustomerRepository(DatabaseContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Customer>> GetAllAsync()
    {
        const string sql = "SELECT * FROM customers ORDER BY CreatedAt DESC";
        return await _context.QueryAsync<Customer>(sql);
    }

    public async Task<Customer?> GetByIdAsync(int id)
    {
        const string sql = "SELECT * FROM customers WHERE Id = @Id";
        return await _context.QueryFirstOrDefaultAsync<Customer>(sql, new { Id = id });
    }

    public async Task<int> CreateAsync(Customer customer)
    {
        const string sql = @"
            INSERT INTO customers (FirstName, LastName, Email, Phone, Address, DateOfBirth, CreatedAt, UpdatedAt)
            VALUES (@FirstName, @LastName, @Email, @Phone, @Address, @DateOfBirth, @CreatedAt, @UpdatedAt);
            SELECT LAST_INSERT_ID();";

        customer.CreatedAt = DateTime.UtcNow;
        customer.UpdatedAt = DateTime.UtcNow;

        return await _context.ExecuteScalarAsync<int>(sql, customer);
    }

    public async Task<bool> UpdateAsync(Customer customer)
    {
        const string sql = @"
            UPDATE customers 
            SET FirstName = @FirstName, LastName = @LastName, Email = @Email, 
                Phone = @Phone, Address = @Address, DateOfBirth = @DateOfBirth, 
                UpdatedAt = @UpdatedAt
            WHERE Id = @Id";

        customer.UpdatedAt = DateTime.UtcNow;

        var rowsAffected = await _context.ExecuteAsync(sql, customer);
        return rowsAffected > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        const string sql = "DELETE FROM customers WHERE Id = @Id";
        var rowsAffected = await _context.ExecuteAsync(sql, new { Id = id });
        return rowsAffected > 0;
    }
} 