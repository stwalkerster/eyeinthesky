namespace EyeInTheSky.Web.Models
{
    using EyeInTheSky.Model.Interfaces;

    public class NewStalkModel : ModelBase
    {
        public IIrcChannel IrcChannel { get; set; }
        public string StalkFlag { get; set; }
        public string WatchChannel { get; set; }
    }
}