using AspNetCore.Common.Models;
using System.ComponentModel.DataAnnotations;

namespace AspNetCore.UserService.Models
{
    public sealed class NewUser : UserBase
    {
        [Required]
        public string Password { get; set; } = null!;
    }
}
