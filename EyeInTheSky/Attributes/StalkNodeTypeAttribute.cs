namespace EyeInTheSky.Attributes
{
    using System;
    
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class StalkNodeTypeAttribute : Attribute
    {
        public string ElementName { get; private set; }

        public StalkNodeTypeAttribute(string elementName)
        {
            this.ElementName = elementName;
        }
    }
}