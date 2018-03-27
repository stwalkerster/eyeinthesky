﻿namespace EyeInTheSky.Model.StalkNodes
{
    using EyeInTheSky.Attributes;
    using EyeInTheSky.Model.Interfaces;
    using EyeInTheSky.Model.StalkNodes.BaseNodes;

    [StalkNodeType("summary")]
    public class SummaryStalkNode : RegexLeafNode
    {
        #region Overrides of StalkNode

        protected override bool? DoMatch(IRecentChange rc, bool forceMatch)
        {
            return this.RegexExpression.Match(rc.EditSummary).Success;
        }
        
        public override string ToString()
        {
            return "(summary:\"" + this.RegexExpression + "\")";
        }
        #endregion
    }
}