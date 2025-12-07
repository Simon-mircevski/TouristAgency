using System.ComponentModel.DataAnnotations;

namespace TouristAgencyShared.Models;

public class Guide
{
    public int Id { get; set; }
    
    [Required]
    [StringLength(100)]
    public string FirstName { get; set; } = string.Empty;
    
    [Required]
    [StringLength(100)]
    public string LastName { get; set; } = string.Empty;
    
    [Required]
    [EmailAddress]
    [StringLength(255)]
    public string Email { get; set; } = string.Empty;
    
    [Required]
    [StringLength(20)]
    public string Phone { get; set; } = string.Empty;
    
    [Required]
    [StringLength(255)]
    public string Specialization { get; set; } = string.Empty;
    
    [Required]
    [StringLength(500)]
    public string Languages { get; set; } = string.Empty;
    
    public int ExperienceYears { get; set; } = 0;
    
    public bool IsAvailable { get; set; } = true;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}