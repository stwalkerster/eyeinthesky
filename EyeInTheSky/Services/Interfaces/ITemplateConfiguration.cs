using System.Collections.Generic;
using EyeInTheSky.Model.Interfaces;

namespace EyeInTheSky.Services.Interfaces
{
    public interface ITemplateConfiguration : IConfigurationBase<ITemplate>
    {
        IStalk NewFromTemplate(string flag, ITemplate template, IList<string> parameters);
    }
}