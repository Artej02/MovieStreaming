using System.ComponentModel.DataAnnotations;

namespace MovieStreaming.Custom.Models.User
{
    public class Login
    {
        [Required]
        public string Username { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public bool RememberLogin { get; set; }
        public string Language { get; set; }
    }
}
