namespace EyeInTheSky.Tests.Model.StalkNodes.NumericNodes
{
    using System.Collections.Generic;

    using EyeInTheSky.Model.Interfaces;
    using EyeInTheSky.Model.StalkNodes.BaseNodes;
    using EyeInTheSky.Model.StalkNodes.NumericNodes;
    using NSubstitute;
    using NUnit.Framework;

    [TestFixture]
    public class InfixNumericLogicalNodeTests
    {
        [Test, TestCaseSource(typeof(InfixNumericLogicalNodeTests), nameof(TestCases))]
        public bool? ProviderTest(int? left, string op, int? right)
        {
            var rc = Substitute.For<IRecentChange>();

            var node = new InfixNumericLogicalNode();
            var leftMock = Substitute.For<INumberProviderNode>();
            leftMock.GetValue(Arg.Any<IRecentChange>(), false).Returns(left);
            var rightMock = Substitute.For<INumberProviderNode>();
            rightMock.GetValue(Arg.Any<IRecentChange>(), false).Returns(right);
            
            node.LeftChildNode = leftMock;
            node.RightChildNode = rightMock;
            node.Operator = op;
            
            return node.Match(rc, false);
        }
        
        private static IEnumerable<TestCaseData> TestCases
        {
            get
            {
                yield return new TestCaseData(5, "==", 5).Returns(true);
                yield return new TestCaseData(5, "==", 4).Returns(false);
                yield return new TestCaseData(4, "==", 5).Returns(false);
                yield return new TestCaseData(-5, "==", -5).Returns(true);
                yield return new TestCaseData(-5, "==", 5).Returns(false);
                yield return new TestCaseData(-5, "==", -4).Returns(false);
                yield return new TestCaseData(-4, "==", -5).Returns(false);
                yield return new TestCaseData(0, "==", 0).Returns(true);
                yield return new TestCaseData(null, "==", 0).Returns(null);
                yield return new TestCaseData(0, "==", null).Returns(null);
                
                yield return new TestCaseData(5, "<>", 5).Returns(false);
                yield return new TestCaseData(5, "<>", 4).Returns(true);
                yield return new TestCaseData(4, "<>", 5).Returns(true);
                yield return new TestCaseData(-5, "<>", -5).Returns(false);
                yield return new TestCaseData(-5, "<>", 5).Returns(true);
                yield return new TestCaseData(-5, "<>", -4).Returns(true);
                yield return new TestCaseData(-4, "<>", -5).Returns(true);
                yield return new TestCaseData(0, "<>", 0).Returns(false);
                yield return new TestCaseData(null, "<>", 0).Returns(null);
                yield return new TestCaseData(0, "<>", null).Returns(null);
                
                yield return new TestCaseData(5, "<", 5).Returns(false);
                yield return new TestCaseData(5, "<", 4).Returns(false);
                yield return new TestCaseData(4, "<", 5).Returns(true);
                yield return new TestCaseData(-5, "<", -5).Returns(false);
                yield return new TestCaseData(-5, "<", 5).Returns(true);
                yield return new TestCaseData(-5, "<", -4).Returns(true);
                yield return new TestCaseData(-4, "<", -5).Returns(false);
                yield return new TestCaseData(0, "<", 0).Returns(false);
                yield return new TestCaseData(null, "<", 0).Returns(null);
                yield return new TestCaseData(0, "<", null).Returns(null);
                
                yield return new TestCaseData(5, ">", 5).Returns(false);
                yield return new TestCaseData(5, ">", 4).Returns(true);
                yield return new TestCaseData(4, ">", 5).Returns(false);
                yield return new TestCaseData(-5, ">", -5).Returns(false);
                yield return new TestCaseData(-5, ">", 5).Returns(false);
                yield return new TestCaseData(-5, ">", -4).Returns(false);
                yield return new TestCaseData(-4, ">", -5).Returns(true);
                yield return new TestCaseData(0, ">", 0).Returns(false);
                yield return new TestCaseData(null, ">", 0).Returns(null);
                yield return new TestCaseData(0, ">", null).Returns(null);
                
                yield return new TestCaseData(5, "<=", 5).Returns(true);
                yield return new TestCaseData(5, "<=", 4).Returns(false);
                yield return new TestCaseData(4, "<=", 5).Returns(true);
                yield return new TestCaseData(-5, "<=", -5).Returns(true);
                yield return new TestCaseData(-5, "<=", 5).Returns(true);
                yield return new TestCaseData(-5, "<=", -4).Returns(true);
                yield return new TestCaseData(-4, "<=", -5).Returns(false);
                yield return new TestCaseData(0, "<=", 0).Returns(true);
                yield return new TestCaseData(null, "<=", 0).Returns(null);
                yield return new TestCaseData(0, "<=", null).Returns(null);
                
                yield return new TestCaseData(5, ">=", 5).Returns(true);
                yield return new TestCaseData(5, ">=", 4).Returns(true);
                yield return new TestCaseData(4, ">=", 5).Returns(false);
                yield return new TestCaseData(-5, ">=", -5).Returns(true);
                yield return new TestCaseData(-5, ">=", 5).Returns(false);
                yield return new TestCaseData(-5, ">=", -4).Returns(false);
                yield return new TestCaseData(-4, ">=", -5).Returns(true);
                yield return new TestCaseData(0, ">=", 0).Returns(true);
                yield return new TestCaseData(null, ">=", 0).Returns(null);
                yield return new TestCaseData(0, ">=", null).Returns(null);
            }
        }
    }
}