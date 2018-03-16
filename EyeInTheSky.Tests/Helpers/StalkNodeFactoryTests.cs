namespace EyeInTheSky.Tests.Helpers
{
    using System.Xml;
    using EyeInTheSky.Helpers;
    using EyeInTheSky.StalkNodes;
    using NUnit.Framework;

    [TestFixture]
    public class StalkNodeFactoryTests
    {
        [Test]
        public void ShouldCreateTrueNode()
        {
            // arrange
            var snf = new StalkNodeFactory();
            var doc = new XmlDocument();
            doc.LoadXml("<true />");

            // act
            var result = snf.NewFromXmlFragment(doc.DocumentElement);

            // assert
            Assert.IsInstanceOf<TrueNode>(result);
        }

        [Test]
        public void ShouldCreateFalseNode()
        {
            // arrange
            var snf = new StalkNodeFactory();
            var doc = new XmlDocument();
            doc.LoadXml("<false />");

            // act
            var result = snf.NewFromXmlFragment(doc.DocumentElement);

            // assert
            Assert.IsInstanceOf<FalseNode>(result);
        }

        [Test]
        public void ShouldCreateUserNode()
        {
            // arrange
            var snf = new StalkNodeFactory();
            var doc = new XmlDocument();
            doc.LoadXml("<user value=\"abc\" />");

            // act
            var result = snf.NewFromXmlFragment(doc.DocumentElement);

            // assert
            Assert.IsInstanceOf<UserStalkNode>(result);
            Assert.AreEqual("abc", ((UserStalkNode) result).GetMatchExpression());
        }

        [Test]
        public void ShouldCreatePageNode()
        {
            // arrange
            var snf = new StalkNodeFactory();
            var doc = new XmlDocument();
            doc.LoadXml("<page value=\"abc\" />");

            // act
            var result = snf.NewFromXmlFragment(doc.DocumentElement);

            // assert
            
            Assert.IsInstanceOf<PageStalkNode>(result);
            Assert.AreEqual("abc", ((PageStalkNode) result).GetMatchExpression());
        }

        [Test]
        public void ShouldCreateSummaryNode()
        {
            // arrange
            var snf = new StalkNodeFactory();
            var doc = new XmlDocument();
            doc.LoadXml("<summary value=\"abc\" />");

            // act
            var result = snf.NewFromXmlFragment(doc.DocumentElement);

            // assert
            Assert.IsInstanceOf<SummaryStalkNode>(result);
            Assert.AreEqual("abc", ((SummaryStalkNode) result).GetMatchExpression());
        }

        [Test]
        public void ShouldCreateFlagNode()
        {
            // arrange
            var snf = new StalkNodeFactory();
            var doc = new XmlDocument();
            doc.LoadXml("<flag value=\"abc\" />");

            // act
            var result = snf.NewFromXmlFragment(doc.DocumentElement);

            // assert
            Assert.IsInstanceOf<FlagStalkNode>(result);
            Assert.AreEqual("abc", ((FlagStalkNode) result).GetMatchExpression());
        }

        [Test]
        public void ShouldCreateNotNode()
        {
            // arrange
            var snf = new StalkNodeFactory();
            var doc = new XmlDocument();
            doc.LoadXml("<not><true /></not>");

            // act
            var result = snf.NewFromXmlFragment(doc.DocumentElement);

            // assert
            Assert.IsInstanceOf<NotNode>(result);
            
            var scln = (SingleChildLogicalNode)result;           
            Assert.IsInstanceOf<TrueNode>(scln.ChildNode);
        }
        
        [Test]
        public void ShouldCreateXorNode()
        {
            // arrange
            var snf = new StalkNodeFactory();
            var doc = new XmlDocument();
            doc.LoadXml("<xor><true /><false /></xor>");

            // act
            var result = snf.NewFromXmlFragment(doc.DocumentElement);

            // assert
            Assert.IsInstanceOf<XorNode>(result);
            
            var scln = (DoubleChildLogicalNode)result;           
            Assert.IsInstanceOf<TrueNode>(scln.LeftChildNode);
            Assert.IsInstanceOf<FalseNode>(scln.RightChildNode);
        }
        
        [Test]
        public void ShouldCreateAndNode()
        {
            // arrange
            var snf = new StalkNodeFactory();
            var doc = new XmlDocument();
            doc.LoadXml("<and><true /><false /></and>");

            // act
            var result = snf.NewFromXmlFragment(doc.DocumentElement);

            // assert
            Assert.IsInstanceOf<AndNode>(result);
            
            var scln = (DoubleChildLogicalNode)result;           
            Assert.IsInstanceOf<TrueNode>(scln.LeftChildNode);
            Assert.IsInstanceOf<FalseNode>(scln.RightChildNode);
        }
        
        [Test]
        public void ShouldCreateOrNode()
        {
            // arrange
            var snf = new StalkNodeFactory();
            var doc = new XmlDocument();
            doc.LoadXml("<or><true /><false /></or>");

            // act
            var result = snf.NewFromXmlFragment(doc.DocumentElement);

            // assert
            Assert.IsInstanceOf<OrNode>(result);
            
            var scln = (DoubleChildLogicalNode)result;           
            Assert.IsInstanceOf<TrueNode>(scln.LeftChildNode);
            Assert.IsInstanceOf<FalseNode>(scln.RightChildNode);
        }
    }
}