using Stwalkerster.IrcClient.Model;

namespace EyeInTheSky.Tests.Model.StalkNodes
{
    using System.Collections;
    using EyeInTheSky.Model;
    using EyeInTheSky.Model.Interfaces;
    using EyeInTheSky.Model.StalkNodes;
    using EyeInTheSky.Model.StalkNodes.BaseNodes;
    using EyeInTheSky.Tests.Model.StalkNodes.BaseNodes;
    using Moq;
    using NUnit.Framework;

    [TestFixture]
    public class XorNodeTest : DoubleChildNodeTestBase<XorNode>
    {
        [Test, TestCaseSource(typeof(XorNodeTest), "TestCases")]
        public bool? DualOperatorTest(IStalkNode a, IStalkNode b, Mock<IStalkNode> tm, Mock<IStalkNode> fm)
        {
            var node = new XorNode();
            node.LeftChildNode = a;
            node.RightChildNode = b;

            tm.Setup(x => x.Match(It.IsAny<IRecentChange>(), It.IsAny<bool>())).Returns(true);
            fm.Setup(x => x.Match(It.IsAny<IRecentChange>(), It.IsAny<bool>())).Returns(false);
            
            return node.Match(new RecentChange(""), false);
        }
        
        [Test, TestCaseSource(typeof(XorNodeTest), "TestCases")]
        public bool? DualOperatorForceTest(IStalkNode a, IStalkNode b, Mock<IStalkNode> tm, Mock<IStalkNode> fm)
        {
            var node = new XorNode();
            node.LeftChildNode = a;
            node.RightChildNode = b;

            tm.Setup(x => x.Match(It.IsAny<IRecentChange>(), true)).Returns(true);
            fm.Setup(x => x.Match(It.IsAny<IRecentChange>(), true)).Returns(false);
            tm.Setup(x => x.Match(It.IsAny<IRecentChange>(), false)).Returns<bool?>(null);
            fm.Setup(x => x.Match(It.IsAny<IRecentChange>(), false)).Returns<bool?>(null);

            return node.Match(new RecentChange(""), true);
        }

        private static IEnumerable TestCases
        {
            get
            {
                var nullNodeMock = new Mock<IStalkNode>();
                nullNodeMock.Setup(x => x.Match(It.IsAny<IRecentChange>())).Returns(null);

                var tm = new Mock<IStalkNode>();
                var fm = new Mock<IStalkNode>();
                
                var t = tm.Object;
                var f = fm.Object;
                var n = nullNodeMock.Object;
                
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