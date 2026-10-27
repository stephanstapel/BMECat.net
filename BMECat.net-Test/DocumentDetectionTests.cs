using BMECat.net;
using System.Text;

namespace BMECat.net_Test
{
    /// <summary>
    /// Tests for detecting the document version and for the namespace injection of documents
    /// without xmlns attribute, which rewrites the first bytes of the document
    /// </summary>
    [TestClass]
    public class DocumentDetectionTests
    {
        private const int DetectionBufferSize = 1024;


        #region Version detection
        [TestMethod]
        public void VersionAttributeWithSingleQuotesIsDetected()
        {
            string xml = "<?xml version='1.0' encoding='utf-8'?>" +
                         "<BMECAT version='2005'>" +
                         "<HEADER><CATALOG><LANGUAGE>deu</LANGUAGE><CATALOG_ID>CAT</CATALOG_ID><CATALOG_VERSION>1.0</CATALOG_VERSION></CATALOG></HEADER>" +
                         "<T_NEW_CATALOG><PRODUCT mode='new'><SUPPLIER_PID>P1</SUPPLIER_PID></PRODUCT></T_NEW_CATALOG>" +
                         "</BMECAT>";

            ProductCatalog catalog = ProductCatalog.Load(new MemoryStream(Encoding.UTF8.GetBytes(xml)));

            Assert.AreEqual(1, catalog.Products.Count, "The document has to be read as BMECat 2005");
            Assert.AreEqual("P1", catalog.Products[0].No);
        } // !VersionAttributeWithSingleQuotesIsDetected()


        [TestMethod]
        public void Version12DocumentIsReadAsVersion12()
        {
            string xml = "<?xml version=\"1.0\" encoding=\"utf-8\"?>" +
                         "<BMECAT version=\"1.2\">" +
                         "<HEADER><CATALOG><LANGUAGE>deu</LANGUAGE><CATALOG_ID>CAT12</CATALOG_ID><CATALOG_VERSION>1.0</CATALOG_VERSION></CATALOG></HEADER>" +
                         "<T_NEW_CATALOG><ARTICLE mode=\"new\"><SUPPLIER_AID>A1</SUPPLIER_AID></ARTICLE></T_NEW_CATALOG>" +
                         "</BMECAT>";

            ProductCatalog catalog = ProductCatalog.Load(new MemoryStream(Encoding.UTF8.GetBytes(xml)));

            Assert.AreEqual("CAT12", catalog.CatalogId);
            Assert.AreEqual(1, catalog.Products.Count);
            Assert.AreEqual("A1", catalog.Products[0].No);
        } // !Version12DocumentIsReadAsVersion12()
        #endregion // !Version detection


        #region Namespace injection
        [TestMethod]
        public void MultiByteCharacterAtTheEndOfTheDetectionBufferIsPreserved()
        {
            string prefix = "<?xml version=\"1.0\" encoding=\"utf-8\"?>" +
                            "<BMECAT version=\"2005\">" +
                            "<HEADER><CATALOG><LANGUAGE>deu</LANGUAGE><CATALOG_ID>CAT</CATALOG_ID><CATALOG_VERSION>1.0</CATALOG_VERSION>" +
                            "<CATALOG_NAME>";

            // place the two byte character ä right on the boundary of the detection buffer, i.e.
            // its first byte is the last byte of the buffer, its second byte the first byte after it
            string catalogName = new string('x', DetectionBufferSize - 1 - Encoding.UTF8.GetByteCount(prefix)) + "ärger";
            string xml = prefix + catalogName + "</CATALOG_NAME></CATALOG></HEADER><T_NEW_CATALOG /></BMECAT>";
            Assert.AreEqual(DetectionBufferSize - 1, Encoding.UTF8.GetByteCount(xml.Substring(0, xml.IndexOf('ä'))));

            ProductCatalog catalog = ProductCatalog.Load(new MemoryStream(Encoding.UTF8.GetBytes(xml)));

            Assert.AreEqual(catalogName, catalog.CatalogName);
        } // !MultiByteCharacterAtTheEndOfTheDetectionBufferIsPreserved()


        [TestMethod]
        public void Latin1DocumentWithoutNamespaceIsRead()
        {
            string xml = "<?xml version=\"1.0\" encoding=\"iso-8859-1\"?>" +
                         "<BMECAT version=\"2005\">" +
                         "<HEADER><CATALOG><LANGUAGE>deu</LANGUAGE><CATALOG_ID>CAT</CATALOG_ID><CATALOG_VERSION>1.0</CATALOG_VERSION>" +
                         "<CATALOG_NAME>Bürobedarf</CATALOG_NAME></CATALOG></HEADER>" +
                         "<T_NEW_CATALOG />" +
                         "</BMECAT>";

            ProductCatalog catalog = ProductCatalog.Load(new MemoryStream(Encoding.Latin1.GetBytes(xml)));

            Assert.AreEqual("Bürobedarf", catalog.CatalogName);
        } // !Latin1DocumentWithoutNamespaceIsRead()


        [TestMethod]
        public void Latin1DocumentWithNamespaceIsRead()
        {
            string xml = "<?xml version=\"1.0\" encoding=\"iso-8859-1\"?>" +
                         "<BMECAT version=\"2005\" xmlns=\"http://www.bmecat.org/bmecat/2005\">" +
                         "<HEADER><CATALOG><LANGUAGE>deu</LANGUAGE><CATALOG_ID>CAT</CATALOG_ID><CATALOG_VERSION>1.0</CATALOG_VERSION>" +
                         "<CATALOG_NAME>Bürobedarf</CATALOG_NAME></CATALOG></HEADER>" +
                         "<T_NEW_CATALOG />" +
                         "</BMECAT>";

            ProductCatalog catalog = ProductCatalog.Load(new MemoryStream(Encoding.Latin1.GetBytes(xml)));

            Assert.AreEqual("Bürobedarf", catalog.CatalogName);
        } // !Latin1DocumentWithNamespaceIsRead()
        #endregion // !Namespace injection


        #region Stream requirements
        [TestMethod]
        public async Task LoadingFromANonSeekableStreamFailsWithIllegalStreamException()
        {
            byte[] buffer = Encoding.UTF8.GetBytes("<BMECAT version=\"2005\"><HEADER /></BMECAT>");

            await Assert.ThrowsExceptionAsync<IllegalStreamException>(() => ProductCatalog.LoadAsync(new NonSeekableStream(buffer)));
        } // !LoadingFromANonSeekableStreamFailsWithIllegalStreamException()
        #endregion // !Stream requirements
    }
}
