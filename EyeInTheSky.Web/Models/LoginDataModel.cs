namespace EyeInTheSky.Web.Models
{
    public class LoginDataModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
        
        public string Error { get; set; }
        public bool HasError
        {
            get { return !string.IsNullOrWhiteSpace(this.Error); }
        }
    }
}