namespace EyeInTheSky.Tests.Model.StalkNodes.NumericNodes
{
    using System.Collections.Generic;

    using EyeInTheSky.Model.Interfaces;
    using EyeInTheSky.Model.StalkNodes.NumericNodes;
    using NSubstitute;
    using NUnit.Framework;

    [TestFixture]
    public class StaticNumberNodeTests
    {
        [Test, TestCaseSource(typeof(StaticNumberNodeTests), nameof(TestCases))]
        public long? ProviderTest(int val)
        {
            var node = new StaticNumberNode();
            node.Value = val;
            
            return node.GetValue(Substitute.For<IRecentChange>(), false);
        }
        
        private static IEnumerable<TestCaseData> TestCases
        {
            get
            {
                yield return new TestCaseData(5).Returns(5);
                yield return new TestCaseData(-6).Returns(-6);
                yield return new TestCaseData(0).Returns(0);
            }
        }
    }
}