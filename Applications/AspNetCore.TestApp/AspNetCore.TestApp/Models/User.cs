using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCore.TestApp.Models
{
    public sealed class User
    {
        [Key]
        public int Id { get; set; }

        [Required]        
        public string Username { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public string Phone{ get; set; }
        
    }
}
