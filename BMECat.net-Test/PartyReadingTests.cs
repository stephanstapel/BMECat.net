using BMECat.net;
using System.Text;

namespace BMECat.net_Test
{
    /// <summary>
    /// Tests for reading buyer and supplier information including their addresses
    /// </summary>
    [TestClass]
    public class PartyReadingTests
    {
        #region BMECat 2005, PARTIES structure
        [TestMethod]
        public void PartyIdAndNameAreAssignedByRole_2005()
        {
            ProductCatalog catalog = _Load(_Document2005WithParties());

            Assert.IsNotNull(catalog.Supplier);
            Assert.AreEqual("SUP-1", catalog.Supplier.Id);
            Assert.AreEqual("Lieferant GmbH", catalog.Supplier.Name);

            Assert.IsNotNull(catalog.Buyer);
            Assert.AreEqual("BUY-1", catalog.Buyer.Id);
            Assert.AreEqual("Kunde AG", catalog.Buyer.Name);
        } // !PartyIdAndNameAreAssignedByRole_2005()


        [TestMethod]
        public void PartyAddressIsRead_2005()
        {
            ProductCatalog catalog = _Load(_Document2005WithParties());

            Assert.AreEqual("Hauptstr. 1", catalog.Supplier.Street);
            Assert.AreEqual("12345", catalog.Supplier.Zip);
            Assert.AreEqual("Berlin", catalog.Supplier.City);
            Assert.AreEqual("DE", catalog.Supplier.Country);
            Assert.AreEqual("DE123456789", catalog.Supplier.VATID);
        } // !PartyAddressIsRead_2005()


        [TestMethod]
        public void PartyAddressNamesAreReadFromTheirOwnElements_2005()
        {
            ProductCatalog catalog = _Load(_Document2005WithParties());

            Assert.AreEqual("Vertrieb", catalog.Supplier.Name2);
            Assert.AreEqual("Haus B", catalog.Supplier.Name3);
        } // !PartyAddressNamesAreReadFromTheirOwnElements_2005()


        [TestMethod]
        public void PartyContactDetailsAreRead_2005()
        {
            ProductCatalog catalog = _Load(_Document2005WithParties());

            Assert.AreEqual(1, catalog.Supplier.ContactDetails.Count);
            Assert.AreEqual("Mustermann", catalog.Supplier.ContactDetails[0].Name);
            Assert.AreEqual("Erika", catalog.Supplier.ContactDetails[0].FirstName);
        } // !PartyContactDetailsAreRead_2005()


        [TestMethod]
        public void AddressOfOnePartyIsNotAssignedToAnotherParty_2005()
        {
            ProductCatalog catalog = _Load(_Document2005WithParties());

            Assert.AreNotEqual(catalog.Supplier.City, catalog.Buyer.City);
            Assert.AreEqual("Hamburg", catalog.Buyer.City);
        } // !AddressOfOnePartyIsNotAssignedToAnotherParty_2005()
        #endregion // !BMECat 2005, PARTIES structure


        #region Legacy BUYER/ SUPPLIER structures
        [TestMethod]
        public void LegacySupplierIsRead_2005()
        {
            string xml = "<?xml version=\"1.0\" encoding=\"utf-8\"?>" +
                         "<BMECAT version=\"2005\" xmlns=\"http://www.bmecat.org/bmecat/2005\">" +
                         "<HEADER>" +
                         "<CATALOG><LANGUAGE>deu</LANGUAGE><CATALOG_ID>CAT</CATALOG_ID><CATALOG_VERSION>1.0</CATALOG_VERSION></CATALOG>" +
                         "<SUPPLIER><SUPPLIER_ID>SUP-LEGACY</SUPPLIER_ID><SUPPLIER_NAME>Legacy GmbH</SUPPLIER_NAME>" +
                         "<ADDRESS><STREET>Altweg 3</STREET><CITY>Köln</CITY></ADDRESS></SUPPLIER>" +
                         "</HEADER>" +
                         "<T_NEW_CATALOG />" +
                         "</BMECAT>";

            ProductCatalog catalog = _Load(xml);

            Assert.AreEqual("SUP-LEGACY", catalog.Supplier.Id);
            Assert.AreEqual("Legacy GmbH", catalog.Supplier.Name);
            Assert.AreEqual("Altweg 3", catalog.Supplier.Street);
            Assert.AreEqual("Köln", catalog.Supplier.City);
        } // !LegacySupplierIsRead_2005()


        [TestMethod]
        public void SupplierIsRead_12()
        {
            string xml = "<?xml version=\"1.0\" encoding=\"utf-8\"?>" +
                         "<BMECAT version=\"1.2\" xmlns=\"http://www.bmecat.org/XMLSchema/1.2/bmecat_new_catalog\">" +
                         "<HEADER>" +
                         "<CATALOG><LANGUAGE>deu</LANGUAGE><CATALOG_ID>CAT</CATALOG_ID><CATALOG_VERSION>1.0</CATALOG_VERSION></CATALOG>" +
                         "<SUPPLIER><SUPPLIER_ID>SUP-12</SUPPLIER_ID><SUPPLIER_NAME>Lieferant 1.2</SUPPLIER_NAME>" +
                         "<ADDRESS type=\"supplier\"><NAME>Lieferant 1.2</NAME><STREET>Ringstr. 7</STREET><ZIP>80331</ZIP><CITY>München</CITY></ADDRESS></SUPPLIER>" +
                         "</HEADER>" +
                         "<T_NEW_CATALOG />" +
                         "</BMECAT>";

            ProductCatalog catalog = _Load(xml);

            Assert.AreEqual("SUP-12", catalog.Supplier.Id);
            Assert.AreEqual("Lieferant 1.2", catalog.Supplier.Name);
            Assert.AreEqual("Ringstr. 7", catalog.Supplier.Street);
            Assert.AreEqual("80331", catalog.Supplier.Zip);
            Assert.AreEqual("München", catalog.Supplier.City);
        } // !SupplierIsRead_12()
        #endregion // !Legacy BUYER/ SUPPLIER structures


        #region Helpers
        private ProductCatalog _Load(string xml)
        {
            return ProductCatalog.Load(new MemoryStream(Encoding.UTF8.GetBytes(xml)));
        } // !_Load()


        private string _Document2005WithParties()
        {
            return "<?xml version=\"1.0\" encoding=\"utf-8\"?>" +
                   "<BMECAT version=\"2005\" xmlns=\"http://www.bmecat.org/bmecat/2005\">" +
                   "<HEADER>" +
                   "<CATALOG><LANGUAGE>deu</LANGUAGE><CATALOG_ID>CAT</CATALOG_ID><CATALOG_VERSION>1.0</CATALOG_VERSION></CATALOG>" +
                   "<PARTIES>" +
                   "<PARTY>" +
                   "<PARTY_ID type=\"supplier_specific\">SUP-1</PARTY_ID>" +
                   "<PARTY_ROLE>supplier</PARTY_ROLE>" +
                   "<ADDRESS type=\"supplier\">" +
                   "<NAME>Lieferant GmbH</NAME><NAME2>Vertrieb</NAME2><NAME3>Haus B</NAME3>" +
                   "<CONTACT_DETAILS><CONTACT_NAME>Mustermann</CONTACT_NAME><FIRST_NAME>Erika</FIRST_NAME></CONTACT_DETAILS>" +
                   "<STREET>Hauptstr. 1</STREET><ZIP>12345</ZIP><CITY>Berlin</CITY><COUNTRY>DE</COUNTRY><VAT_ID>DE123456789</VAT_ID>" +
                   "</ADDRESS>" +
                   "</PARTY>" +
                   "<PARTY>" +
                   "<PARTY_ID type=\"buyer_specific\">BUY-1</PARTY_ID>" +
                   "<PARTY_ROLE>buyer</PARTY_ROLE>" +
                   "<ADDRESS type=\"buyer\"><NAME>Kunde AG</NAME><STREET>Hafenstr. 2</STREET><ZIP>20457</ZIP><CITY>Hamburg</CITY></ADDRESS>" +
                   "</PARTY>" +
                   "</PARTIES>" +
                   "</HEADER>" +
                   "<T_NEW_CATALOG />" +
                   "</BMECAT>";
        } // !_Document2005WithParties()
        #endregion // !Helpers
    }
}
