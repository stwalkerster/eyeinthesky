namespace EyeInTheSky.StalkNodes
{
    using System.Linq;
    using EyeInTheSky.Attributes;
    using EyeInTheSky.Model.Interfaces;
    
    [StalkNodeType("usergroup")]
    public class UserGroupStalkNode : LeafNode
    {
        protected override bool? DoMatch(IRecentChange rc, bool forceMatch)
        {
            if (!forceMatch)
            {
                return null;
            }

            return rc.GetUserGroups().Contains(this.Expression);
        }

        public override string ToString()
        {
            return string.Format("(group:{0})", this.Expression);
        }
    }
}