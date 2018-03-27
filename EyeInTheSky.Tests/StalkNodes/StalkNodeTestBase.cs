namespace EyeInTheSky.Tests.StalkNodes
{
    using System;
    using System.Collections.Generic;
    using EyeInTheSky.Model.Interfaces;
    using EyeInTheSky.StalkNodes;
    using Moq;
    using NUnit.Framework;

    public abstract class MultiChildNodeTestBase<T> : LogicalNodeTestBase<T> where T : MultiChildLogicalNode, new()
    {
        [Test]
        public void ShouldRejectNullNodeMatch()
        {
            var node = new T {ChildNodes = null};
            var rc = new Mock<IRecentChange> {DefaultValueProvider = new ValueProvider()};

            Assert.Catch(typeof(InvalidOperationException), () => node.Match(rc.Object));
        }
        
        [Test]
        public void ShouldRejectEmptyNodeListMatch()
        {    
            var node = new T {ChildNodes = new List<IStalkNode>()};
            var rc = new Mock<IRecentChange> {DefaultValueProvider = new ValueProvider()};

            Assert.Catch(typeof(InvalidOperationException), () => node.Match(rc.Object));
        }
    }
    
    public abstract class DoubleChildNodeTestBase<T> : LogicalNodeTestBase<T> where T : DoubleChildLogicalNode, new()
    {
        [Test]
        public void ShouldRejectSingleLeftNodeMatch()
        {
            var node = new T {LeftChildNode = new TrueNode()};
            var rc = new Mock<IRecentChange> {DefaultValueProvider = new ValueProvider()};

            Assert.Catch(typeof(InvalidOperationException), () => node.Match(rc.Object));
        }
        
        [Test]
        public void ShouldRejectSingleRightNodeMatch()
        {
            var node = new T {RightChildNode = new TrueNode()};
            var rc = new Mock<IRecentChange> {DefaultValueProvider = new ValueProvider()};

            Assert.Catch(typeof(InvalidOperationException), () => node.Match(rc.Object));
        }
    }
    
    public abstract class SingleChildNodeTestBase<T> : LogicalNodeTestBase<T> where T : SingleChildLogicalNode, new()
    {
        [Test]
        public void ShouldRejectSingleRightNodeMatch()
        {
            var node = new T();
            var rc = new Mock<IRecentChange> {DefaultValueProvider = new ValueProvider()};

            Assert.Catch(typeof(InvalidOperationException), () => node.Match(rc.Object));
        }
    }
    
    public abstract class LogicalNodeTestBase<T> : StalkNodeTestBase<T> where T : LogicalNode, new()
    {   
    }

    public abstract class LeafNodeTestBase<T> : StalkNodeTestBase<T> where T : LeafNode, new()
    {
    }
    
    public abstract class RegexLeafNodeTestBase<T> : LeafNodeTestBase<T> where T : RegexLeafNode, new()
    {
        [Test]
        public void ShouldRejectNoDefinitionMatch()
        {
            var node = new T();
            var rc = new Mock<IRecentChange> {DefaultValueProvider = new ValueProvider()};

            Assert.Catch(typeof(InvalidOperationException), () => node.Match(rc.Object));
        }
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
    }
}