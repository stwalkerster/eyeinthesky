﻿namespace EyeInTheSky.Tests.Model.StalkNodes
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using EyeInTheSky.Model;
    using EyeInTheSky.Model.Interfaces;
    using EyeInTheSky.Model.StalkNodes;
    using EyeInTheSky.Model.StalkNodes.BaseNodes;
    using EyeInTheSky.Tests.Model.StalkNodes.BaseNodes;
    using NSubstitute;
    using NUnit.Framework;

    [TestFixture]
    public class XOfStalkNodeTests : MultiChildNodeTestBase<XOfStalkNode>
    {
        [Test, TestCaseSource(typeof(XOfStalkNodeTests), nameof(MatchCorrectlyTestData))]
        public bool? ShouldMatchCorrectly(int? min, int? max, List<IStalkNode> nodes)
        {
            // arrange
            var node = new XOfStalkNode();
            node.ChildNodes.AddRange(nodes);

            node.Minimum = min;
            node.Maximum = max;

            TestContext.Out.Write("Nodelist: " + string.Join(",", nodes));

            // act, assert
            return node.Match(new RecentChange(""), false);
        }

        public static IEnumerable MatchCorrectlyTestData
        {
            get
            {
                var t = new TrueNode();
                var f = new FalseNode();
                var nullMock = Substitute.For<IStalkNode>();
                nullMock.Match(Arg.Any<IRecentChange>(), false).Returns((bool?) null);

                var n = nullMock;
                
                // min
                yield return new TestCaseData(0, null, new List<IStalkNode> {f}).Returns(true);
                yield return new TestCaseData(0, null, new List<IStalkNode> {t}).Returns(true);
                yield return new TestCaseData(0, null, new List<IStalkNode> {n}).Returns(true);

                yield return new TestCaseData(1, null, new List<IStalkNode> {f}).Returns(false);
                yield return new TestCaseData(1, null, new List<IStalkNode> {t}).Returns(true);
                yield return new TestCaseData(1, null, new List<IStalkNode> {n}).Returns(null);

                yield return new TestCaseData(2, null, new List<IStalkNode> {f}).Returns(false);
                yield return new TestCaseData(2, null, new List<IStalkNode> {t}).Returns(false);
                yield return new TestCaseData(2, null, new List<IStalkNode> {n}).Returns(false);
                
                // max
                yield return new TestCaseData(null, 0, new List<IStalkNode> {f}).Returns(true);
                yield return new TestCaseData(null, 0, new List<IStalkNode> {t}).Returns(false);
                yield return new TestCaseData(null, 0, new List<IStalkNode> {n}).Returns(null);

                yield return new TestCaseData(null, 1, new List<IStalkNode> {f}).Returns(true);
                yield return new TestCaseData(null, 1, new List<IStalkNode> {t}).Returns(true);
                yield return new TestCaseData(null, 1, new List<IStalkNode> {n}).Returns(true);

                yield return new TestCaseData(null, 2, new List<IStalkNode> {f}).Returns(true);
                yield return new TestCaseData(null, 2, new List<IStalkNode> {t}).Returns(true);
                yield return new TestCaseData(null, 2, new List<IStalkNode> {n}).Returns(true);
                
                // multi
                yield return new TestCaseData(2, 4, new List<IStalkNode> {t, f, f, f, f}).Returns(false);
                yield return new TestCaseData(2, 4, new List<IStalkNode> {t, n, f, f, f}).Returns(null);
                yield return new TestCaseData(2, 4, new List<IStalkNode> {t, t, t, t, n}).Returns(null);
                yield return new TestCaseData(2, 3, new List<IStalkNode> {t, n, n, f, f}).Returns(null);
                yield return new TestCaseData(2, 3, new List<IStalkNode> {t, t, n, f, f}).Returns(true);
            }
        }
        
        [Test]
        public void ShouldThrowErrorOnNoConditions()
        {
            var node = new XOfStalkNode();
            node.ChildNodes.Add(new FalseNode());

            Assert.Throws<InvalidOperationException>(() => node.Match(this.RecentChangeBuilder()));
        }
    }
}