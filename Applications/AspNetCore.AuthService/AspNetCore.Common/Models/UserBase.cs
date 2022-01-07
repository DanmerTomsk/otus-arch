using System.ComponentModel.DataAnnotations;

namespace AspNetCore.Common.Models
{
    public abstract class UserBase
    {
        [Required]
        public string Username { get; set; } = null!;

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        public string? Phone { get; set; }

        public int? RoleId { get; set; }

        public Role? Role { get; set; }
    }
}
