using System;

namespace MovieStreaming.Custom.Models.PasswordReset
{
    public class RequestResetPassword
    {
        public int Id { get; set; }
        public string Username { get; set; }

        public string ResetToken { get; set; }

        public DateTime ExpiryDate { get; set; }
        public DateTime CreatedDate { get; set; }

    }
}
