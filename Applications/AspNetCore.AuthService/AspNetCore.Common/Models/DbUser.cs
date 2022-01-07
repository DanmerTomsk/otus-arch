using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace AspNetCore.Common.Models
{
    public sealed class DbUser : UserBase
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [JsonIgnore]
        public int PasswordHash { get; set; }
    }
}
