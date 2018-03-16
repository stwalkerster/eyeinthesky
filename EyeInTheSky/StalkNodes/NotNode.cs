﻿namespace EyeInTheSky.StalkNodes
{
    using EyeInTheSky.Attributes;
    using EyeInTheSky.Model.Interfaces;

    [StalkNodeType("not")]
    public class NotNode : SingleChildLogicalNode
    {
        #region Overrides of StalkNode

        protected override bool DoMatch(IRecentChange rc)
        {
            return !this.ChildNode.Match(rc);
        }

        public override string ToString()
        {
            return "(!" + this.ChildNode +  ")";
        }
        #endregion
    }
}