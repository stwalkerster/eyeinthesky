namespace EyeInTheSky.Tests.Model.StalkNodes.NumericNodes
{
    using System.Collections.Generic;
    using EyeInTheSky.Model.Interfaces;
    using EyeInTheSky.Model.StalkNodes.NumericNodes;
    using NSubstitute;
    using NUnit.Framework;

    [TestFixture]
    public class DiffDeltaNumberNodeTests
    {
        [Test, TestCaseSource(typeof(DiffDeltaNumberNodeTests), nameof(TestCases))]
        public long? ProviderTest(int? val)
        {
            var rc = Substitute.For<IRecentChange>();
            rc.SizeDiff.Returns(val);

            var node = new DiffDeltaNumberNode();
            return node.GetValue(rc, false);
        }
        
        private static IEnumerable<TestCaseData> TestCases
        {
            get
            {
                yield return new TestCaseData(5).Returns(5);
                yield return new TestCaseData(-6).Returns(-6);
                yield return new TestCaseData(0).Returns(0);
                yield return new TestCaseData(null).Returns(null);
            }
        }
    }
}