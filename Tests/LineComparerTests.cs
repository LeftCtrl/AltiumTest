using NUnit.Framework;
using Shared;
using Sorter;

namespace Tests
{
    class LineComparerTests
    {
        private LineComparer _lineComparer = new LineComparer();

        [TestCase("99", "ABCD", "100", "ABCE")]
        [TestCase("100", "ABCD", "100", "ABCE")]
        [TestCase("101", "ABCD", "100", "ABCE")]
        [TestCase("99", "ABC", "100", "ABCE")]
        [TestCase("100", "ABC", "100", "ABCE")]
        [TestCase("101", "ABC", "100", "ABCE")]
        [TestCase("99", "ABCDA", "100", "ABCE")]
        [TestCase("100", "ABCDA", "100", "ABCE")]
        [TestCase("101", "ABCDA", "100", "ABCE")]
        [TestCase("99", "ABCD", "100", "ABCEA")]
        [TestCase("100", "ABCD", "100", "ABCEA")]
        [TestCase("101", "ABCD", "100", "ABCEA")]
        public void ShouldCompareStringPartsFirst(string num1, string str1, string num2, string str2)
        {
            string x = GetLine(num1, str1);
            string y = GetLine(num2, str2);

            Assert.Negative(_lineComparer.Compare(x, y));
            Assert.Positive(_lineComparer.Compare(y, x));
        }

        [TestCase("1", "200", "ABCD")]
        [TestCase("2", "200", "ABCD")]
        [TestCase("9", "200", "ABCD")]
        [TestCase("100", "200", "ABCD")]
        [TestCase("199", "200", "ABCD")]
        public void ShouldCompareNumberPartWhenStringPartsAreEqual(string num1, string num2, string str)
        {
            string x = GetLine(num1, str);
            string y = GetLine(num2, str);

            Assert.Negative(_lineComparer.Compare(x, y));
            Assert.Positive(_lineComparer.Compare(y, x));
        }

        [TestCase("100", "ABCD")]
        public void ShouldReturnZeroWhenLinesAreEqual(string num, string str)
        {
            string x = GetLine(num, str);
            string y = GetLine(num, str);

            int result = _lineComparer.Compare(x, y);
            Assert.AreEqual(0, result);
        }

        [TestCase("100", "ABCD", "100", "ABCE")]
        public void ShouldUseFirstDelimeterInLine(string num1, string str1, string num2, string str2)
        {
            string xCheck = GetLine(num1, str1);
            string yCheck = GetLine(num2, str2);
            int check = _lineComparer.Compare(xCheck, yCheck);

            string x = GetLine(num1, $"{str1}{Settings.Delimiter}{str2}");
            string y = GetLine(num2, $"{str2}{Settings.Delimiter}{str1}");
            int result = _lineComparer.Compare(xCheck, yCheck);

            Assert.AreEqual(check, result);
        }

        private string GetLine(string num, string str)
        {
            return $"{num}{Settings.Delimiter}{str}";
        }
    }
}
