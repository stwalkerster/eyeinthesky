namespace EyeInTheSky.Tests.Services
{
    using System.Collections;
    using EyeInTheSky.Services;
    using NUnit.Framework;

    [TestFixture]
    public class BooleanParserTests
    {
        [Test, TestCaseSource(typeof(BooleanParserTests), nameof(ParseCorrectlyTestData))]
        public bool ShouldParseCorrectly(string input)
        {
            bool result;
            bool success = BooleanParser.TryParse(input, out result);

            Assert.IsTrue(success);
            return result;
        }

        public static IEnumerable ParseCorrectlyTestData
        {
            get
            {
                yield return new TestCaseData("true").Returns(true);
                yield return new TestCaseData("false").Returns(false);
                yield return new TestCaseData("  true  ").Returns(true);
                yield return new TestCaseData("  false  ").Returns(false);
                yield return new TestCaseData("yes").Returns(true);
                yield return new TestCaseData("no").Returns(false);
                yield return new TestCaseData("on").Returns(true);
                yield return new TestCaseData("off").Returns(false);
                yield return new TestCaseData("1").Returns(true);
                yield return new TestCaseData("0").Returns(false);
            }
        }
        
        [Test, TestCaseSource(typeof(BooleanParserTests), nameof(ParseIncorrectlyTestData))]
        public void ShouldParseIncorrectly(string input)
        {
            bool result;
            Assert.IsFalse(BooleanParser.TryParse(input, out result));
        }

        public static IEnumerable ParseIncorrectlyTestData
        {
            get
            {
                yield return new TestCaseData("tr ue");
                yield return new TestCaseData("fa lse");
                yield return new TestCaseData("");
                yield return new TestCaseData("banana");
                yield return new TestCaseData(null);
                yield return new TestCaseData("2");
            }
        }

    }
}