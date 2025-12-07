using System.ComponentModel.DataAnnotations;

namespace TouristAgencyShared.Models;

public class Booking
{
    public int Id { get; set; }
    
    [Required]
    public int CustomerId { get; set; }
    
    [Required]
    public int TourId { get; set; }
    
    [Required]
    public DateTime BookingDate { get; set; } = DateTime.UtcNow;
    
    [Required]
    public DateTime TravelDate { get; set; }
    
    [Required]
    [Range(1, int.MaxValue)]
    public int NumberOfPeople { get; set; } = 1;
    
    [Required]
    [Range(0, double.MaxValue)]
    public decimal TotalAmount { get; set; }
    
    [Required]
    public string Status { get; set; } = "Pending"; // Pending, Confirmed, Cancelled, Completed
    
    public string? SpecialRequests { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public Customer? Customer { get; set; }
    public Tour? Tour { get; set; }
}