namespace EyeInTheSky.Web.Models
{
    using System.Collections.Generic;
    using EyeInTheSky.Model.Interfaces;

    public class GetUserListModel : ModelBase
    {
        public List<IBotUser> BotUsers { get; set; }
    }
}