using System.ComponentModel.DataAnnotations;

namespace TouristAgencyShared.Models;

public class Tour
{
    public int Id { get; set; }
    
    [Required]
    [StringLength(255)]
    public string Name { get; set; } = string.Empty;
    
    public string? Description { get; set; }
    
    [Required]
    public int DestinationId { get; set; }
    
    public int? GuideId { get; set; }
    
    [Required]
    public DateTime StartDate { get; set; }
    
    [Required]
    public DateTime EndDate { get; set; }
    
    [Required]
    [Range(1, int.MaxValue)]
    public int MaxParticipants { get; set; } = 20;
    
    [Required]
    [Range(0, int.MaxValue)]
    public int CurrentParticipants { get; set; } = 0;
    
    [Required]
    [Range(0, double.MaxValue)]
    public decimal Price { get; set; }
    
    [Required]
    public string Status { get; set; } = "Scheduled"; // Scheduled, Active, Completed, Cancelled
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public Destination? Destination { get; set; }
    public Guide? Guide { get; set; }
}