namespace EyeInTheSky.Tests.Model.StalkNodes.BaseNodes
{
    using System;
    using System.Collections.Generic;
    using EyeInTheSky.Model.Interfaces;
    using EyeInTheSky.Model.StalkNodes;
    using EyeInTheSky.Model.StalkNodes.BaseNodes;
    using NSubstitute;
    using NUnit.Framework;

    public abstract class MultiChildNodeTestBase<T> : LogicalNodeTestBase<T> where T : MultiChildLogicalNode, new()
    {
        [Test]
        public void ShouldRejectNullNodeMatch()
        {
            var node = new T {ChildNodes = null};
            var rc = this.RecentChangeBuilder();

            Assert.Catch(typeof(InvalidOperationException), () => node.Match(rc));
        }
        
        [Test]
        public void ShouldRejectEmptyNodeListMatch()
        {
            var node = new T {ChildNodes = new List<IStalkNode>()};
            var rc = this.RecentChangeBuilder();

            Assert.Catch(typeof(InvalidOperationException), () => node.Match(rc));
        }

        [Test]
        public void ShouldCloneSuccessfully()
        {
            // arrange
            var orig = new T();
            orig.ChildNodes.Add(new TrueNode());
            orig.ChildNodes.Add(new FalseNode());
            orig.ChildNodes.Add(new TrueNode());

            // act
            var cloned = (MultiChildLogicalNode) orig.Clone();

            // assert
            Assert.AreNotSame(orig, cloned);
            Assert.AreNotSame(orig.ChildNodes[0], cloned.ChildNodes[0]);
            Assert.AreNotSame(orig.ChildNodes[1], cloned.ChildNodes[1]);
            Assert.AreNotSame(orig.ChildNodes[2], cloned.ChildNodes[2]);
            Assert.AreEqual(orig.ToString(), cloned.ToString());
        }
    }
    
    public abstract class DoubleChildNodeTestBase<T> : LogicalNodeTestBase<T> where T : DoubleChildLogicalNode, new()
    {
        [Test]
        public void ShouldRejectSingleLeftNodeMatch()
        {
            var node = new T {LeftChildNode = new TrueNode()};
            var rc = this.RecentChangeBuilder();

            Assert.Catch(typeof(InvalidOperationException), () => node.Match(rc));
        }
        
        [Test]
        public void ShouldRejectSingleRightNodeMatch()
        {
            var node = new T {RightChildNode = new TrueNode()};
            var rc = this.RecentChangeBuilder();

            Assert.Catch(typeof(InvalidOperationException), () => node.Match(rc));
        }
        
        [Test]
        public void ShouldCloneSuccessfully()
        {
            // arrange
            var orig = new T();
            orig.LeftChildNode = new TrueNode();
            orig.RightChildNode = new FalseNode();

            // act
            var cloned = (DoubleChildLogicalNode) orig.Clone();

            // assert
            Assert.AreNotSame(orig, cloned);
            Assert.AreNotSame(orig.LeftChildNode, cloned.LeftChildNode);
            Assert.AreNotSame(orig.RightChildNode, cloned.RightChildNode);
            Assert.AreEqual(orig.ToString(), cloned.ToString());
        }
    }
    
    public abstract class SingleChildNodeTestBase<T> : LogicalNodeTestBase<T> where T : SingleChildLogicalNode, new()
    {
        [Test]
        public void ShouldRejectSingleRightNodeMatch()
        {
            var node = new T();
            var rc = this.RecentChangeBuilder();

            Assert.Catch(typeof(InvalidOperationException), () => node.Match(rc));
        }
        
        [Test]
        public void ShouldCloneSuccessfully()
        {
            // arrange
            var orig = new T();
            orig.ChildNode = new TrueNode();

            // act
            var cloned = (SingleChildLogicalNode) orig.Clone();

            // assert
            Assert.AreNotSame(orig, cloned);
            Assert.AreNotSame(orig.ChildNode, cloned.ChildNode);
            Assert.AreEqual(orig.ToString(), cloned.ToString());
        }
    }
    
    public abstract class LogicalNodeTestBase<T> : StalkNodeTestBase<T> where T : LogicalNode, new()
    {   
    }

    public abstract class LeafNodeTestBase<T> : StalkNodeTestBase<T> where T : LeafNode, new()
    {
        
        [Test]
        public void ShouldCloneSuccessfully()
        {
            // arrange
            var orig = new T();
            orig.SetMatchExpression("fooo");

            // act
            var cloned = (IStalkNode) orig.Clone();

            // assert
            Assert.AreNotSame(orig, cloned);
            Assert.AreEqual(orig.ToString(), cloned.ToString());
        }
        
        [Test]
        public void ShouldRejectNoDefinitionMatch()
        {
            var node = new T();
            var rc = this.RecentChangeBuilder();

            Assert.Catch(typeof(InvalidOperationException), () => node.Match(rc));
        }
    }
    
    public abstract class RegexLeafNodeTestBase<T> : LeafNodeTestBase<T> where T : RegexLeafNode, new()
    {
    }
    
    public abstract class StalkNodeTestBase<T> : TestBase where T : StalkNode, new()
    {
        [Test]
        public void ShouldRejectNullChange()
        {
            var node = new T();
            
            Assert.Catch(typeof(ArgumentNullException), () => node.Match(null));
        }

        [Test]
        public void ShouldReturnCslString()
        {
            var node = new T();

            var result = node.ToString();
            
            Assert.AreEqual("(", result.Substring(0, 1));
            Assert.AreEqual(")", result.Substring(result.Length - 1, 1));
        }

        public IRecentChange RecentChangeBuilder()
        {
            var rc = Substitute.For<IRecentChange>();
            rc.EditFlags.Returns("mno");
            rc.EditSummary.Returns("jkl");
            rc.Log.Returns("pqr");
            rc.Page.Returns("abc");
            rc.SizeDiff.Returns(123);
            rc.Url.Returns("ghi");
            rc.User.Returns("def");
            rc.AlternateTargetUser.Returns("alttgt");
            rc.PageIsInCategory(Arg.Any<string>()).Returns(true);

            return rc;
        }
    }
}