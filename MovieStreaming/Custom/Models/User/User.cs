using System;
using System.ComponentModel.DataAnnotations;

namespace MovieStreaming.Custom.Models.User
{
    public class User
    {
        [Key]
        public int? Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]

        public string Surname { get; set; }

        [Required]
        public string Username { get; set; }

        public string Password { get; set; }
        public string Salt { get; set; }
        public int? RoleId { get; set; }
        public bool? IsApproved { get; set; }
        public int? CreatedUserId { get; set; }
        public int? UpdatedUserId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}
