namespace EyeInTheSky.Web.Models
{
    using System.Collections.Generic;

    public class AboutModel : ModelBase
    {
        public IDictionary<string, string> Other { get; set; }
        public IDictionary<string, string> Core { get; set; }
    }
}