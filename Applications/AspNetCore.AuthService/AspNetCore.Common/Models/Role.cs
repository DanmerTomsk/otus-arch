using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace AspNetCore.Common.Models
{
    public class Role
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = null!;

        [JsonIgnore]
        public List<DbUser> Users { get; set; }

        public Role()
        {
            Users = new List<DbUser>();
        }
    }
}
