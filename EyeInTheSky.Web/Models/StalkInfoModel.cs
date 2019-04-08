namespace EyeInTheSky.Web.Models
{
    using EyeInTheSky.Model.Interfaces;
    using EyeInTheSky.Web.Misc;

    public class StalkInfoModel : ModelBase
    {
        public IIrcChannel IrcChannel { get; set; }
        public DisplayStalk Stalk { get; set; }
    }

    public class EditableStalkInfoModel : StalkInfoModel
    {
    }
}