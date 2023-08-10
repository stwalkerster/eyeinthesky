namespace EyeInTheSky.Tests.Model.StalkNodes
{
    using System.Collections;
    using EyeInTheSky.Model;
    using EyeInTheSky.Model.Interfaces;
    using EyeInTheSky.Model.StalkNodes;
    using EyeInTheSky.Model.StalkNodes.BaseNodes;
    using EyeInTheSky.Tests.Model.StalkNodes.BaseNodes;
    using NSubstitute;
    using NUnit.Framework;

    [TestFixture]
    public class XorNodeTest : DoubleChildNodeTestBase<XorNode>
    {
        [Test, TestCaseSource(typeof(XorNodeTest), nameof(TestCases))]
        public bool? DualOperatorTest(IStalkNode a, IStalkNode b, IStalkNode tm, IStalkNode fm)
        {
            var node = new XorNode();
            node.LeftChildNode = a;
            node.RightChildNode = b;

            tm.Match(Arg.Any<IRecentChange>(), Arg.Any<bool>()).Returns(true);
            fm.Match(Arg.Any<IRecentChange>(), Arg.Any<bool>()).Returns(false);
            
            return node.Match(new RecentChange(""), false);
        }
        
        [Test, TestCaseSource(typeof(XorNodeTest), nameof(TestCases))]
        public bool? DualOperatorForceTest(IStalkNode a, IStalkNode b, IStalkNode tm, IStalkNode fm)
        {
            var node = new XorNode();
            node.LeftChildNode = a;
            node.RightChildNode = b;

            tm.Match(Arg.Any<IRecentChange>(), true).Returns(true);
            fm.Match(Arg.Any<IRecentChange>(), true).Returns(false);
            tm.Match(Arg.Any<IRecentChange>(), false).Returns((bool?)null);
            fm.Match(Arg.Any<IRecentChange>(), false).Returns((bool?)null);

            return node.Match(new RecentChange(""), true);
        }

        private static IEnumerable TestCases
        {
            get
            {
                var nullNodeMock = Substitute.For<IStalkNode>();
                nullNodeMock.Match(Arg.Any<IRecentChange>(), false).Returns((bool?)null);


                var tm = Substitute.For<IStalkNode>();
                var fm = Substitute.For<IStalkNode>();
                
                var t = tm;
                var f = fm;
                var n = nullNodeMock;
                
                yield return new TestCaseData(t, t, tm, fm).Returns(false);
                yield return new TestCaseData(f, t, tm, fm).Returns(true);
                yield return new TestCaseData(t, f, tm, fm).Returns(true);
                yield return new TestCaseData(f, f, tm, fm).Returns(false);

                
                yield return new TestCaseData(t, n, tm, fm).Returns(null);
                yield return new TestCaseData(f, n, tm, fm).Returns(null);
                yield return new TestCaseData(n, t, tm, fm).Returns(null);
                yield return new TestCaseData(n, f, tm, fm).Returns(null);
                yield return new TestCaseData(n, n, tm, fm).Returns(null);
            }
        }
    }
}