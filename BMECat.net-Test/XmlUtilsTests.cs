using BMECat.net;
using System.Globalization;
using System.Xml;

namespace BMECat.net_Test
{
    /// <summary>
    /// Tests for the typed node accessors of XmlUtils
    /// </summary>
    [TestClass]
    public class XmlUtilsTests
    {
        /// <summary>
        /// Buddhist calendar, i.e. the year 2004 is written as 2547
        /// </summary>
        private static readonly CultureInfo THAI = new CultureInfo("th-TH");


        #region Dates
        [TestMethod]
        public void DateWithoutTimeIsReadUsingGregorianCalendar_ThaiCulture()
        {
            XmlNode node = _Node("<DATE>2004-08-20</DATE>");

            DateTime? date = _RunWithCulture(THAI, () => XmlUtils.nodeAsDateTime(node, "."));

            Assert.AreEqual(new DateTime(2004, 8, 20), date);
        } // !DateWithoutTimeIsReadUsingGregorianCalendar_ThaiCulture()


        [TestMethod]
        public void DateTimeWithoutOffsetIsReadAsTwentyFourHourTime_ThaiCulture()
        {
            XmlNode node = _Node("<DATE>2004-08-20T14:59:54</DATE>");

            DateTime? date = _RunWithCulture(THAI, () => XmlUtils.nodeAsDateTime(node, "."));

            Assert.AreEqual(new DateTime(2004, 8, 20, 14, 59, 54), date);
        } // !DateTimeWithoutOffsetIsReadAsTwentyFourHourTime_ThaiCulture()


        [TestMethod]
        public void GermanDateIsReadDayFirst()
        {
            Assert.AreEqual(new DateTime(2004, 2, 1), XmlUtils.nodeAsDateTime(_Node("<DATE>01.02.2004</DATE>"), "."));
            Assert.AreEqual(new DateTime(2004, 8, 20), XmlUtils.nodeAsDateTime(_Node("<DATE>20.08.2004</DATE>"), "."));
            Assert.AreEqual(new DateTime(2004, 8, 20, 14, 59, 54), XmlUtils.nodeAsDateTime(_Node("<DATE>20.08.2004 14:59:54</DATE>"), "."));
        } // !GermanDateIsReadDayFirst()


        [TestMethod]
        public void DateTimeWithFractionalSecondsIsRead()
        {
            XmlNode node = _Node("<DATE>2004-08-20T14:59:54.123</DATE>");

            Assert.AreEqual(new DateTime(2004, 8, 20, 14, 59, 54, 123), XmlUtils.nodeAsDateTime(node, "."));
        } // !DateTimeWithFractionalSecondsIsRead()


        [TestMethod]
        public void CompactDateIsRead()
        {
            XmlNode node = _Node("<DATE>20040820</DATE>");

            Assert.AreEqual(new DateTime(2004, 8, 20), XmlUtils.nodeAsDateTime(node, "."));
        } // !CompactDateIsRead()


        [TestMethod]
        public void InvalidDateReturnsDefaultValue()
        {
            XmlNode node = _Node("<DATE>tomorrow</DATE>");

            Assert.IsNull(XmlUtils.nodeAsDateTime(node, "."));
            Assert.AreEqual(DateTime.MinValue, XmlUtils.nodeAsDateTime(node, ".", defaultValue: DateTime.MinValue));
        } // !InvalidDateReturnsDefaultValue()
        #endregion // !Dates


        #region Booleans
        [TestMethod]
        [DataRow("true", true)]
        [DataRow(" TRUE ", true)]
        [DataRow("1", true)]
        [DataRow("false", false)]
        [DataRow("0", false)]
        [DataRow("yes", false)]
        public void BooleanValuesAreRead(string value, bool expected)
        {
            XmlNode node = _Node($"<FLAG>{value}</FLAG>");

            Assert.AreEqual(expected, XmlUtils.nodeAsBool(node, "."));
        } // !BooleanValuesAreRead()


        [TestMethod]
        public void MissingBooleanReturnsDefaultValue()
        {
            XmlNode node = _Node("<ROOT />");

            Assert.IsTrue(XmlUtils.nodeAsBool(node, "./FLAG"));
            Assert.IsFalse(XmlUtils.nodeAsBool(node, "./FLAG", defaultValue: false));
        } // !MissingBooleanReturnsDefaultValue()
        #endregion // !Booleans


        #region Numbers and strings
        [TestMethod]
        public void IntegerWithSurroundingWhitespaceIsRead()
        {
            Assert.AreEqual(42, XmlUtils.nodeAsInt(_Node("<N> 42 </N>"), "."));
        } // !IntegerWithSurroundingWhitespaceIsRead()


        [TestMethod]
        public void NonIntegerReturnsDefaultValue()
        {
            Assert.IsNull(XmlUtils.nodeAsInt(_Node("<N>4.2</N>"), "."));
            Assert.AreEqual(-1, XmlUtils.nodeAsInt(_Node("<N>abc</N>"), ".", defaultValue: -1));
        } // !NonIntegerReturnsDefaultValue()


        [TestMethod]
        public void InvalidXPathReturnsDefaultValue()
        {
            XmlNode node = _Node("<ROOT><A>x</A></ROOT>");

            // the prefix is not declared, which makes the evaluation fail with an XPathException
            Assert.AreEqual("fallback", XmlUtils.nodeAsString(node, "./undeclared:A", null, "fallback"));
            Assert.IsNull(XmlUtils.nodesAsStrings(node, "./undeclared:A"));
        } // !InvalidXPathReturnsDefaultValue()


        [TestMethod]
        public void NodesAsStringsReturnsAllValuesInDocumentOrder()
        {
            XmlNode node = _Node("<ROOT><V>b</V><V>a</V><V>c</V></ROOT>");

            CollectionAssert.AreEqual(new List<string>() { "b", "a", "c" }, XmlUtils.nodesAsStrings(node, "./V"));
        } // !NodesAsStringsReturnsAllValuesInDocumentOrder()


        [TestMethod]
        public void MissingAttributeReturnsDefaultText()
        {
            XmlNode node = _Node("<ROOT type=\"x\" />");

            Assert.AreEqual("x", XmlUtils.AttributeText(node, "type"));
            Assert.AreEqual("", XmlUtils.AttributeText(node, "mode"));
            Assert.AreEqual("new", XmlUtils.AttributeText(node, "mode", "new"));
        } // !MissingAttributeReturnsDefaultText()
        #endregion // !Numbers and strings


        #region Helpers
        private XmlNode _Node(string xml)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);
            return doc.DocumentElement!;
        } // !_Node()


        private T _RunWithCulture<T>(CultureInfo culture, Func<T> function)
        {
            CultureInfo previousCulture = CultureInfo.CurrentCulture;

            try
            {
                CultureInfo.CurrentCulture = culture;
                return function();
            }
            finally
            {
                CultureInfo.CurrentCulture = previousCulture;
            }
        } // !_RunWithCulture()
        #endregion // !Helpers
    }
}
