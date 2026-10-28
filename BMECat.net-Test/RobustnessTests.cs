using BMECat.net;
using System.Text;
using System.Xml;
using System.Xml.XPath;

namespace BMECat.net_Test
{
    /// <summary>
    /// Tests for handling of resources, incomplete stream reads and error propagation.
    /// </summary>
    [TestClass]
    public class RobustnessTests
    {
        #region Resource handling
        [TestMethod]
        public void LoadingFromFileReleasesTheFileHandle()
        {
            string path = Path.GetTempFileName();

            try
            {
                _GenerateCatalog().Save(path);

                ProductCatalog catalog = ProductCatalog.Load(path);
                Assert.AreEqual(1, catalog.Products.Count);

                // fails with an IOException as long as the reader keeps the file stream open
                File.Delete(path);
                Assert.IsFalse(File.Exists(path));
            }
            finally
            {
                if (File.Exists(path))
                {
                    try { File.Delete(path); } catch { }
                }
            }
        } // !LoadingFromFileReleasesTheFileHandle()


        [TestMethod]
        public void FailingLoadFromFileReleasesTheFileHandle()
        {
            string path = Path.GetTempFileName();

            try
            {
                File.WriteAllText(path, "<BMECAT version=\"2005\"><HEADER>this is not valid xml");

                try
                {
                    ProductCatalog.Load(path);
                    Assert.Fail("Loading a malformed document has to fail");
                }
                catch (Exception ex) when (!(ex is AssertFailedException))
                {
                    // the synchronous entry point wraps the XmlException into an AggregateException
                }

                // fails with an IOException as long as the reader keeps the file stream open
                File.Delete(path);
                Assert.IsFalse(File.Exists(path));
            }
            finally
            {
                if (File.Exists(path))
                {
                    try { File.Delete(path); } catch { }
                }
            }
        } // !FailingLoadFromFileReleasesTheFileHandle()
        #endregion // !Resource handling


        #region Incomplete stream reads
        [TestMethod]
        public void CatalogIsReadFromAStreamThatReturnsFewBytesPerRead()
        {
            byte[] buffer = Encoding.UTF8.GetBytes(_WriteCatalog(_GenerateCatalog()));

            // e.g. a network stream that hands out its data in small chunks
            ProductCatalog catalog = ProductCatalog.Load(new DripStream(buffer, 16));

            Assert.AreEqual("QA_CAT_002", catalog.CatalogId);
            Assert.AreEqual(1, catalog.Products.Count);
            Assert.AreEqual(16.49m, catalog.Products.First().Prices.First().Amount);
        } // !CatalogIsReadFromAStreamThatReturnsFewBytesPerRead()


        [TestMethod]
        public void SmallCatalogWithoutNamespaceIsRead()
        {
            // less than 1024 bytes, i.e. the version detection buffer is only partially filled
            string xml = "<?xml version=\"1.0\" encoding=\"utf-8\"?>" +
                         "<BMECAT version=\"2005\">" +
                         "<HEADER><CATALOG><CATALOG_ID>SMALL_CAT</CATALOG_ID><CATALOG_VERSION>001.001</CATALOG_VERSION></CATALOG></HEADER>" +
                         "<T_NEW_CATALOG></T_NEW_CATALOG>" +
                         "</BMECAT>";
            Assert.IsTrue(xml.Length < 1024);

            ProductCatalog catalog = ProductCatalog.Load(new MemoryStream(Encoding.UTF8.GetBytes(xml)));

            Assert.AreEqual("SMALL_CAT", catalog.CatalogId);
        } // !SmallCatalogWithoutNamespaceIsRead()
        #endregion // !Incomplete stream reads


        #region Error propagation
        [TestMethod]
        public void OriginalStackTraceIsPreserved()
        {
            ThrowingXmlDocument doc = new ThrowingXmlDocument();
            doc.LoadXml("<BMECAT><HEADER /></BMECAT>");

            Exception caughtException = null;
            try
            {
                XmlUtils.nodeAsString(doc.DocumentElement, "./HEADER");
            }
            catch (Exception ex)
            {
                caughtException = ex;
            }

            Assert.IsNotNull(caughtException, "The exception has to be propagated to the caller");
            StringAssert.Contains(caughtException.StackTrace ?? String.Empty, "CreateNavigator", "Rethrowing must not reset the stack trace of the original exception");
        } // !OriginalStackTraceIsPreserved()
        #endregion // !Error propagation


        #region Helpers
        /// <summary>
        /// A document that lets every xpath evaluation fail with a non xpath exception
        /// </summary>
        private class ThrowingXmlDocument : XmlDocument
        {
            protected override XPathNavigator CreateNavigator(XmlNode node)
            {
                throw new InvalidOperationException("navigator not available");
            }
        } // !class ThrowingXmlDocument


        /// <summary>
        /// A stream that returns only a few bytes per read call, just like e.g. a network stream does
        /// </summary>
        private class DripStream : MemoryStream
        {
            private readonly int _maxBytesPerRead;

            public DripStream(byte[] buffer, int maxBytesPerRead)
                : base(buffer, false)
            {
                _maxBytesPerRead = maxBytesPerRead;
            }

            public override int Read(byte[] buffer, int offset, int count)
            {
                return base.Read(buffer, offset, Math.Min(count, _maxBytesPerRead));
            }
        } // !class DripStream


        private string _WriteCatalog(ProductCatalog catalog)
        {
            MemoryStream ms = new MemoryStream();
            catalog.Save(ms);
            return Encoding.UTF8.GetString(ms.ToArray());
        } // !_WriteCatalog()


        private ProductCatalog _GenerateCatalog()
        {
            ProductCatalog catalog = new ProductCatalog()
            {
                Languages = { LanguageCodes.DEU },
                CatalogId = "QA_CAT_002",
                CatalogVersion = "001.002",
                CatalogName = "Office Material",
                GenerationDate = new DateTime(2004, 8, 20, 10, 59, 54),
                Currency = CurrencyCodes.EUR
            };

            catalog.Products.Add(new Product()
            {
                No = "Q20-P09",
                PIds = new List<ProductId>() { new ProductId() { Type = ProductIdTypes.EAN, Id = "0000000011" } },
                DescriptionShort = "Post-Safe Polythene Envelopes",
                Stock = 100,
                Prices = new List<ProductPrice>()
                {
                    new ProductPrice()
                    {
                        Currency = CurrencyCodes.EUR,
                        PriceType = PriceTypes.NetList,
                        Amount = 16.49m,
                        Tax = 0.19m
                    }
                }
            });

            return catalog;
        } // !_GenerateCatalog()
        #endregion // !Helpers
    }
}
