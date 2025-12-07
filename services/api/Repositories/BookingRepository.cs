using TouristAgencyAPI.Data;
using TouristAgencyAPI.Repositories;
using TouristAgencyShared.Models;
using Dapper;

namespace TouristAgencyAPI.Repositories;

public class BookingRepository : IRepository<Booking>
{
    private readonly DatabaseContext _context;

    public BookingRepository(DatabaseContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Booking>> GetAllAsync()
    {
        const string sql = @"
            SELECT b.*, c.*, t.*
            FROM bookings b
            LEFT JOIN customers c ON b.CustomerId = c.Id
            LEFT JOIN tours t ON b.TourId = t.Id
            ORDER BY b.CreatedAt DESC";

        var bookingDictionary = new Dictionary<int, Booking>();
        
        using var connection = _context.CreateConnection();
        await connection.QueryAsync<Booking, Customer, Tour, Booking>(
            sql,
            (booking, customer, tour) =>
            {
                if (!bookingDictionary.TryGetValue(booking.Id, out var bookingEntry))
                {
                    bookingEntry = booking;
                    bookingDictionary.Add(booking.Id, bookingEntry);
                }
                
                bookingEntry.Customer = customer;
                bookingEntry.Tour = tour;
                
                return bookingEntry;
            },
            splitOn: "Id");

        return bookingDictionary.Values;
    }

    public async Task<Booking?> GetByIdAsync(int id)
    {
        const string sql = @"
            SELECT b.*, c.*, t.*
            FROM bookings b
            LEFT JOIN customers c ON b.CustomerId = c.Id
            LEFT JOIN tours t ON b.TourId = t.Id
            WHERE b.Id = @Id";

        Booking? result = null;
        
        using var connection = _context.CreateConnection();
        await connection.QueryAsync<Booking, Customer, Tour, Booking>(
            sql,
            (booking, customer, tour) =>
            {
                if (result == null)
                {
                    result = booking;
                }
                
                result.Customer = customer;
                result.Tour = tour;
                
                return result;
            },
            new { Id = id },
            splitOn: "Id");

        return result;
    }

    public async Task<int> CreateAsync(Booking booking)
    {
        const string sql = @"
            INSERT INTO bookings (CustomerId, TourId, BookingDate, TravelDate, NumberOfPeople, 
                                 TotalAmount, Status, SpecialRequests, CreatedAt, UpdatedAt)
            VALUES (@CustomerId, @TourId, @BookingDate, @TravelDate, @NumberOfPeople, 
                    @TotalAmount, @Status, @SpecialRequests, @CreatedAt, @UpdatedAt);
            SELECT LAST_INSERT_ID();";

        booking.CreatedAt = DateTime.UtcNow;
        booking.UpdatedAt = DateTime.UtcNow;

        return await _context.ExecuteScalarAsync<int>(sql, booking);
    }

    public async Task<bool> UpdateAsync(Booking booking)
    {
        const string sql = @"
            UPDATE bookings 
            SET CustomerId = @CustomerId, TourId = @TourId, BookingDate = @BookingDate, 
                TravelDate = @TravelDate, NumberOfPeople = @NumberOfPeople, 
                TotalAmount = @TotalAmount, Status = @Status, SpecialRequests = @SpecialRequests, 
                UpdatedAt = @UpdatedAt
            WHERE Id = @Id";

        booking.UpdatedAt = DateTime.UtcNow;

        var rowsAffected = await _context.ExecuteAsync(sql, booking);
        return rowsAffected > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        const string sql = "DELETE FROM bookings WHERE Id = @Id";
        var rowsAffected = await _context.ExecuteAsync(sql, new { Id = id });
        return rowsAffected > 0;
    }
} 