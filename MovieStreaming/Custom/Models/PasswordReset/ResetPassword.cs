namespace MovieStreaming.Custom.Models.PasswordReset
{
    public class ResetPassword
    {
        public ResetPassword(string resetToken)
        {
            this.resetToken = resetToken;
        }
        public ResetPassword()
        {

        }
        public string resetToken { get; set; }

        public string Password { get; set; }

        public string ConfirmPassword { get; set; }


    }
}
