namespace EyeInTheSky.Model.StalkNodes.BaseNodes
{
    using System.Text.RegularExpressions;

    public abstract class RegexLeafNode : LeafNode
    {
        protected Regex RegexExpression;
        protected string rawRegexExpression;
        private bool caseInsensitive;

        public bool CaseInsensitive
        {
            get { return this.caseInsensitive; }
            set
            {
                this.caseInsensitive = value;
                this.RegexExpression = null;
            }
        }

        protected void Compile()
        {
            if (this.RegexExpression != null)
            {
                return;
            }

            var options = RegexOptions.None;
            if (this.CaseInsensitive)
            {
                options |= RegexOptions.IgnoreCase;
            }

            this.RegexExpression = new Regex(this.rawRegexExpression, options);
        }

        public override void SetMatchExpression(string regex)
        {
            this.rawRegexExpression = regex;
            this.RegexExpression = null;
            base.SetMatchExpression(regex);
        }

        public override string GetMatchExpression()
        {
            return this.rawRegexExpression;
        }
    }
}