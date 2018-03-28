namespace EyeInTheSky.Tests.Services
{
    using System.Collections.Generic;
    using System.Xml;
    using EyeInTheSky.Model.StalkNodes;
    using EyeInTheSky.Model.StalkNodes.BaseNodes;
    using EyeInTheSky.Services;
    using NUnit.Framework;

    [TestFixture]
    public class StalkNodeFactoryTests
    {
        #region Object tests
        
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
        public void ShouldCreateUserGroupNode()
        {
            // arrange
            var snf = new StalkNodeFactory();
            var doc = new XmlDocument();
            doc.LoadXml("<usergroup value=\"abc\" />");

            // act
            var result = snf.NewFromXmlFragment(doc.DocumentElement);

            // assert
            Assert.IsInstanceOf<UserGroupStalkNode>(result);
            Assert.AreEqual("abc", ((UserGroupStalkNode) result).GetMatchExpression());
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
            
            var mcln = (MultiChildLogicalNode)result;    
            Assert.AreEqual(2, mcln.ChildNodes.Count);
            Assert.IsInstanceOf<TrueNode>(mcln.ChildNodes[0]);
            Assert.IsInstanceOf<FalseNode>(mcln.ChildNodes[1]);
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
            
            var mcln = (MultiChildLogicalNode)result;           
            Assert.AreEqual(2, mcln.ChildNodes.Count);
            Assert.IsInstanceOf<TrueNode>(mcln.ChildNodes[0]);
            Assert.IsInstanceOf<FalseNode>(mcln.ChildNodes[1]);
        }
        
        [Test]
        public void ShouldCreateBasicXOfNode()
        {
            // arrange
            var snf = new StalkNodeFactory();
            var doc = new XmlDocument();
            doc.LoadXml("<x-of><true /><false /></x-of>");

            // act
            var result = snf.NewFromXmlFragment(doc.DocumentElement);

            // assert
            Assert.IsInstanceOf<XOfStalkNode>(result);
            
            var mcln = (XOfStalkNode)result;
            Assert.IsNull(mcln.Minimum);
            Assert.IsNull(mcln.Maximum);
            Assert.AreEqual(2, mcln.ChildNodes.Count);
            Assert.IsInstanceOf<TrueNode>(mcln.ChildNodes[0]);
            Assert.IsInstanceOf<FalseNode>(mcln.ChildNodes[1]);
        }
        
        [Test]
        public void ShouldCreateMinXOfNode()
        {
            // arrange
            var snf = new StalkNodeFactory();
            var doc = new XmlDocument();
            doc.LoadXml("<x-of minimum=\"5\"><true /><false /></x-of>");

            // act
            var result = snf.NewFromXmlFragment(doc.DocumentElement);

            // assert
            Assert.IsInstanceOf<XOfStalkNode>(result);
            
            var mcln = (XOfStalkNode)result;
            Assert.AreEqual(5, mcln.Minimum);
            Assert.IsNull(mcln.Maximum);
                       
            Assert.AreEqual(2, mcln.ChildNodes.Count);
            Assert.IsInstanceOf<TrueNode>(mcln.ChildNodes[0]);
            Assert.IsInstanceOf<FalseNode>(mcln.ChildNodes[1]);
        }
        
        [Test]
        public void ShouldCreateMaxXOfNode()
        {
            // arrange
            var snf = new StalkNodeFactory();
            var doc = new XmlDocument();
            doc.LoadXml("<x-of maximum=\"5\"><true /><false /></x-of>");

            // act
            var result = snf.NewFromXmlFragment(doc.DocumentElement);

            // assert
            Assert.IsInstanceOf<XOfStalkNode>(result);
            
            var mcln = (XOfStalkNode)result;
            Assert.AreEqual(5, mcln.Maximum);
            Assert.IsNull(mcln.Minimum);
                       
            Assert.AreEqual(2, mcln.ChildNodes.Count);
            Assert.IsInstanceOf<TrueNode>(mcln.ChildNodes[0]);
            Assert.IsInstanceOf<FalseNode>(mcln.ChildNodes[1]);
        }
        #endregion
        
        #region XML tests

        [Test]
        public void ShouldSerialiseTrueCorrectly()
        {
            // arrange 
            var node = new TrueNode();
            var doc = new XmlDocument();
            var ns = string.Empty;
            
            var snf = new StalkNodeFactory(); 
            
            // act
            var result = snf.ToXml(doc, ns, node);

            // assert
            Assert.AreEqual("<true />", result.OuterXml);
        }
        
        [Test]
        public void ShouldSerialiseFalseCorrectly()
        {
            // arrange 
            var node = new FalseNode();
            var doc = new XmlDocument();
            var ns = string.Empty;
            
            var snf = new StalkNodeFactory(); 
            
            // act
            var result = snf.ToXml(doc, ns, node);

            // assert
            Assert.AreEqual("<false />", result.OuterXml);
        }
        
        [Test]
        public void ShouldSerialiseUserCorrectly()
        {
            // arrange 
            var node = new UserStalkNode();
            node.SetMatchExpression("abc");
            
            var doc = new XmlDocument();
            var ns = string.Empty;
            
            var snf = new StalkNodeFactory(); 
            
            // act
            var result = snf.ToXml(doc, ns, node);

            // assert
            Assert.AreEqual("<user value=\"abc\" />", result.OuterXml);
        }
        
        [Test]
        public void ShouldSerialisePageCorrectly()
        {
            // arrange 
            var node = new PageStalkNode();
            node.SetMatchExpression("abc");
            
            var doc = new XmlDocument();
            var ns = string.Empty;
            
            var snf = new StalkNodeFactory(); 
            
            // act
            var result = snf.ToXml(doc, ns, node);

            // assert
            Assert.AreEqual("<page value=\"abc\" />", result.OuterXml);
        }
        
        [Test]
        public void ShouldSerialiseSummaryCorrectly()
        {
            // arrange 
            var node = new SummaryStalkNode();
            node.SetMatchExpression("abc");
            
            var doc = new XmlDocument();
            var ns = string.Empty;
            
            var snf = new StalkNodeFactory(); 
            
            // act
            var result = snf.ToXml(doc, ns, node);

            // assert
            Assert.AreEqual("<summary value=\"abc\" />", result.OuterXml);
        }
        
        [Test]
        public void ShouldSerialiseFlagCorrectly()
        {
            // arrange 
            var node = new FlagStalkNode();
            node.SetMatchExpression("abc");
            
            var doc = new XmlDocument();
            var ns = string.Empty;
            
            var snf = new StalkNodeFactory(); 
            
            // act
            var result = snf.ToXml(doc, ns, node);

            // assert
            Assert.AreEqual("<flag value=\"abc\" />", result.OuterXml);
        }
        
        [Test]
        public void ShouldSerialiseUserGroupCorrectly()
        {
            // arrange 
            var node = new UserGroupStalkNode();
            node.SetMatchExpression("abc");
            
            var doc = new XmlDocument();
            var ns = string.Empty;
            
            var snf = new StalkNodeFactory(); 
            
            // act
            var result = snf.ToXml(doc, ns, node);

            // assert
            Assert.AreEqual("<usergroup value=\"abc\" />", result.OuterXml);
        }
        
        [Test]
        public void ShouldSerialiseAndCorrectly()
        {
            // arrange 
            var node = new AndNode
            {
                ChildNodes = new List<IStalkNode>
                {
                    new TrueNode(),
                    new FalseNode()
                }
            };

            var doc = new XmlDocument();
            var ns = string.Empty;
            
            var snf = new StalkNodeFactory(); 
            
            // act
            var result = snf.ToXml(doc, ns, node);

            // assert
            Assert.AreEqual("<and><true /><false /></and>", result.OuterXml);
        }
        
        [Test]
        public void ShouldSerialiseOrCorrectly()
        {
            // arrange 
            var node = new OrNode
            {
                ChildNodes = new List<IStalkNode>
                {
                    new TrueNode(),
                    new FalseNode()
                }
            };

            var doc = new XmlDocument();
            var ns = string.Empty;
            
            var snf = new StalkNodeFactory(); 
            
            // act
            var result = snf.ToXml(doc, ns, node);

            // assert
            Assert.AreEqual("<or><true /><false /></or>", result.OuterXml);
        }
        
        [Test]
        public void ShouldSerialiseXorCorrectly()
        {
            // arrange 
            var node = new XorNode
            {
                LeftChildNode = new TrueNode(),
                RightChildNode = new FalseNode()
            };

            var doc = new XmlDocument();
            var ns = string.Empty;
            
            var snf = new StalkNodeFactory(); 
            
            // act
            var result = snf.ToXml(doc, ns, node);

            // assert
            Assert.AreEqual("<xor><true /><false /></xor>", result.OuterXml);
        }
        
        [Test]
        public void ShouldSerialiseNotCorrectly()
        {
            // arrange 
            var node = new NotNode
            {
                ChildNode = new TrueNode()
            };

            var doc = new XmlDocument();
            var ns = string.Empty;
            
            var snf = new StalkNodeFactory(); 
            
            // act
            var result = snf.ToXml(doc, ns, node);

            // assert
            Assert.AreEqual("<not><true /></not>", result.OuterXml);
        }
        
        [Test]
        public void ShouldSerialiseBasicXOfCorrectly()
        {
            // arrange 
            var node = new XOfStalkNode
            {
                ChildNodes = new List<IStalkNode>
                {
                    new TrueNode(),
                    new FalseNode()
                }
            };

            var doc = new XmlDocument();
            var ns = string.Empty;
            
            var snf = new StalkNodeFactory(); 
            
            // act
            var result = snf.ToXml(doc, ns, node);

            // assert
            Assert.AreEqual("<x-of><true /><false /></x-of>", result.OuterXml);
        }
        
        [Test]
        public void ShouldSerialiseMinXOfCorrectly()
        {
            // arrange 
            var node = new XOfStalkNode
            {
                ChildNodes = new List<IStalkNode>
                {
                    new TrueNode(),
                    new FalseNode()
                },
                Minimum = 5
            };

            var doc = new XmlDocument();
            var ns = string.Empty;
            
            var snf = new StalkNodeFactory(); 
            
            // act
            var result = snf.ToXml(doc, ns, node);

            // assert
            Assert.AreEqual("<x-of minimum=\"5\"><true /><false /></x-of>", result.OuterXml);
        }
        
        [Test]
        public void ShouldSerialiseMaxXOfCorrectly()
        {
            // arrange 
            var node = new XOfStalkNode
            {
                ChildNodes = new List<IStalkNode>
                {
                    new TrueNode(),
                    new FalseNode()
                },
                Maximum = 5
            };

            var doc = new XmlDocument();
            var ns = string.Empty;
            
            var snf = new StalkNodeFactory(); 
            
            // act
            var result = snf.ToXml(doc, ns, node);

            // assert
            Assert.AreEqual("<x-of maximum=\"5\"><true /><false /></x-of>", result.OuterXml);
        }
        
        [Test]
        public void ShouldSerialiseBothXOfCorrectly()
        {
            // arrange 
            var node = new XOfStalkNode
            {
                ChildNodes = new List<IStalkNode>
                {
                    new TrueNode(),
                    new FalseNode()
                },
                Maximum = 5,
                Minimum = 2
            };

            var doc = new XmlDocument();
            var ns = string.Empty;
            
            var snf = new StalkNodeFactory(); 
            
            // act
            var result = snf.ToXml(doc, ns, node);

            // assert
            Assert.AreEqual("<x-of minimum=\"2\" maximum=\"5\"><true /><false /></x-of>", result.OuterXml);
        }
        
        #endregion
    }
}