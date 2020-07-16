namespace EyeInTheSky.Model.StalkNodes
{
    using EyeInTheSky.Attributes;
    using EyeInTheSky.Model.Interfaces;
    using EyeInTheSky.Model.StalkNodes.BaseNodes;

    [StalkNodeType("additionaldata")]
    public class AdditionalDataNode: RegexLeafNode
    {
        protected override bool? DoMatch(IRecentChange rc, bool forceMatch)
        {
            this.Compile();
            
            if (rc.AdditionalData != null)
            {
                return this.RegexExpression.Match(rc.AdditionalData).Success;
            }

            return false;
        }
        
        public override string ToString()
        {
            return "(additionaldata:\"" + this.rawRegexExpression + "\")";
        }
    }
}