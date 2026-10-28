using BMECat.net;
using System.Text;

namespace BMECat.net_Test
{
    /// <summary>
    /// Tests for reading products, their features and the catalog group structure
    /// </summary>
    [TestClass]
    public class ProductReadingTests
    {
        private const string Namespace2005 = "http://www.bmecat.org/bmecat/2005";
        private const string Namespace12 = "http://www.bmecat.org/XMLSchema/1.2/bmecat_new_catalog";


        #region Product order
        [TestMethod]
        public void ProductOrderOfTheDocumentIsPreserved_2005()
        {
            List<string> productNumbers = Enumerable.Range(0, 500).Select(i => $"P{i:0000}").ToList();
            string products = String.Concat(productNumbers.Select(no => $"<PRODUCT mode=\"new\"><SUPPLIER_PID>{no}</SUPPLIER_PID></PRODUCT>"));

            ProductCatalog catalog = _Load(_Document2005(products));

            CollectionAssert.AreEqual(productNumbers, catalog.Products.Select(p => p.No).ToList());
        } // !ProductOrderOfTheDocumentIsPreserved_2005()


        [TestMethod]
        public void ProductOrderOfTheDocumentIsPreserved_12()
        {
            List<string> productNumbers = Enumerable.Range(0, 500).Select(i => $"A{i:0000}").ToList();
            string articles = String.Concat(productNumbers.Select(no => $"<ARTICLE mode=\"new\"><SUPPLIER_AID>{no}</SUPPLIER_AID></ARTICLE>"));

            ProductCatalog catalog = _Load(_Document12(articles));

            CollectionAssert.AreEqual(productNumbers, catalog.Products.Select(p => p.No).ToList());
        } // !ProductOrderOfTheDocumentIsPreserved_12()
        #endregion // !Product order


        #region Product details
        [TestMethod]
        public void ProductDetailsAreRead_2005()
        {
            string product = "<PRODUCT mode=\"new\">" +
                             "<SUPPLIER_PID>P1</SUPPLIER_PID>" +
                             "<PRODUCT_DETAILS>" +
                             "<DESCRIPTION_SHORT>Schraube &amp; Mutter</DESCRIPTION_SHORT>" +
                             "<INTERNATIONAL_PID type=\"gtin\">4012345678901</INTERNATIONAL_PID>" +
                             "<MANUFACTURER_PID>M-1</MANUFACTURER_PID>" +
                             "<MANUFACTURER_NAME>Hersteller</MANUFACTURER_NAME>" +
                             "<KEYWORD>Schraube</KEYWORD><KEYWORD>Mutter</KEYWORD>" +
                             "</PRODUCT_DETAILS>" +
                             "<PRODUCT_ORDER_DETAILS><ORDER_UNIT>C62</ORDER_UNIT><CONTENT_UNIT>C62</CONTENT_UNIT><NO_CU_PER_OU>100</NO_CU_PER_OU><QUANTITY_MIN>2</QUANTITY_MIN></PRODUCT_ORDER_DETAILS>" +
                             "<PRODUCT_PRICE_DETAILS>" +
                             "<PRODUCT_PRICE price_type=\"net_list\"><PRICE_AMOUNT>9.95</PRICE_AMOUNT><PRICE_CURRENCY>EUR</PRICE_CURRENCY><TAX>0.19</TAX></PRODUCT_PRICE>" +
                             "<PRODUCT_PRICE price_type=\"net_list\"><PRICE_AMOUNT>8.50</PRICE_AMOUNT><PRICE_CURRENCY>EUR</PRICE_CURRENCY><TAX>0.19</TAX><LOWER_BOUND>10</LOWER_BOUND></PRODUCT_PRICE>" +
                             "</PRODUCT_PRICE_DETAILS>" +
                             "</PRODUCT>";

            Product p = _Load(_Document2005(product)).Products.Single();

            Assert.AreEqual("Schraube & Mutter", p.DescriptionShort);
            Assert.AreEqual("M-1", p.ManufacturerPID);
            Assert.AreEqual("Hersteller", p.ManufacturerName);
            CollectionAssert.AreEqual(new List<string>() { "Schraube", "Mutter" }, p.Keywords);

            Assert.AreEqual(ProductIdTypes.GTIN, p.PIds.Single().Type);
            Assert.AreEqual("4012345678901", p.PIds.Single().Id);

            Assert.AreEqual(QuantityCodes.C62, p.OrderDetails.OrderUnit.Code);
            Assert.AreEqual("C62", p.OrderDetails.OrderUnit.ClearText);
            Assert.AreEqual(100, p.OrderDetails.ContentUnitPerOrderUnit);
            Assert.AreEqual(2, p.OrderDetails.QuantityMin);

            Assert.AreEqual(2, p.Prices.Count);
            Assert.AreEqual(PriceTypes.NetList, p.Prices[0].PriceType);
            Assert.AreEqual(CurrencyCodes.EUR, p.Prices[0].Currency);
            Assert.AreEqual(9.95m, p.Prices[0].Amount);
            Assert.IsNull(p.Prices[0].LowerBound);
            Assert.AreEqual(8.50m, p.Prices[1].Amount);
            Assert.AreEqual(10, p.Prices[1].LowerBound);
        } // !ProductDetailsAreRead_2005()


        [TestMethod]
        public void ArticleDetailsAreRead_12()
        {
            string article = "<ARTICLE mode=\"new\">" +
                             "<SUPPLIER_AID>A1</SUPPLIER_AID>" +
                             "<ARTICLE_DETAILS><DESCRIPTION_SHORT>Kugelschreiber</DESCRIPTION_SHORT><EAN>4012345678901</EAN><SUPPLIER_ALT_AID>ALT-1</SUPPLIER_ALT_AID></ARTICLE_DETAILS>" +
                             "<ARTICLE_PRICE_DETAILS><ARTICLE_PRICE price_type=\"net_customer\"><PRICE_AMOUNT>1.25</PRICE_AMOUNT><PRICE_CURRENCY>EUR</PRICE_CURRENCY><TAX>0.19</TAX></ARTICLE_PRICE></ARTICLE_PRICE_DETAILS>" +
                             "</ARTICLE>";

            Product p = _Load(_Document12(article)).Products.Single();

            Assert.AreEqual("A1", p.No);
            Assert.AreEqual("Kugelschreiber", p.DescriptionShort);
            Assert.AreEqual("ALT-1", p.SupplierAltPid);
            Assert.AreEqual(ProductIdTypes.EAN, p.PIds.Single().Type);
            Assert.AreEqual("4012345678901", p.PIds.Single().Id);
            Assert.AreEqual(1.25m, p.Prices.Single().Amount);
            Assert.AreEqual(0.19m, p.Prices.Single().Tax);
        } // !ArticleDetailsAreRead_12()


        [TestMethod]
        public void EanIsNotDuplicatedWhenAlsoGivenAsInternationalPid_2005()
        {
            string product = "<PRODUCT mode=\"new\"><SUPPLIER_PID>P1</SUPPLIER_PID><PRODUCT_DETAILS>" +
                             "<INTERNATIONAL_PID type=\"ean\">4012345678901</INTERNATIONAL_PID>" +
                             "<EAN>4012345678901</EAN>" +
                             "</PRODUCT_DETAILS></PRODUCT>";

            Product p = _Load(_Document2005(product)).Products.Single();

            Assert.AreEqual(1, p.PIds.Count);
            Assert.AreEqual(ProductIdTypes.EAN, p.PIds[0].Type);
        } // !EanIsNotDuplicatedWhenAlsoGivenAsInternationalPid_2005()
        #endregion // !Product details


        #region Features
        [TestMethod]
        public void FeaturesAndClassificationAreRead_2005()
        {
            string product = "<PRODUCT mode=\"new\"><SUPPLIER_PID>P1</SUPPLIER_PID>" +
                             "<PRODUCT_FEATURES>" +
                             "<REFERENCE_FEATURE_SYSTEM_NAME>ETIM-9.0</REFERENCE_FEATURE_SYSTEM_NAME>" +
                             "<REFERENCE_FEATURE_GROUP_ID>EC000001</REFERENCE_FEATURE_GROUP_ID>" +
                             "<FEATURE><FNAME>EF000008</FNAME><FVALUE>12.5</FVALUE><FUNIT>MMT</FUNIT></FEATURE>" +
                             "<FEATURE><FNAME>EF000020</FNAME><FVALUE>EV000001</FVALUE><FVALUE>EV000002</FVALUE></FEATURE>" +
                             "</PRODUCT_FEATURES>" +
                             "</PRODUCT>";

            Product p = _Load(_Document2005(product)).Products.Single();

            FeatureSet featureSet = p.FeatureSets.Single();
            Assert.AreEqual("ETIM-9.0", featureSet.FeatureClassificationSystem.Classification);
            Assert.AreEqual("EC000001", featureSet.FeatureClassificationSystem.GroupIds.Single().Name);

            Feature length = p.GetFeature("ef000008")!;
            Assert.IsNotNull(length, "Feature lookup ignores the case by default");
            Assert.AreEqual("12.5", length.Values.Single());
            Assert.AreEqual(QuantityCodes.MMT, length.Unit.Code);

            Feature multiValued = p.GetFeature("EF000020", ignoreCase: false)!;
            CollectionAssert.AreEqual(new List<string>() { "EV000001", "EV000002" }, multiValued.Values);
        } // !FeaturesAndClassificationAreRead_2005()


        [TestMethod]
        public void RemoveFeatureRemovesTheFeatureFromAllFeatureSets()
        {
            string product = "<PRODUCT mode=\"new\"><SUPPLIER_PID>P1</SUPPLIER_PID>" +
                             "<PRODUCT_FEATURES><FEATURE><FNAME>Farbe</FNAME><FVALUE>rot</FVALUE></FEATURE><FEATURE><FNAME>Gewicht</FNAME><FVALUE>1</FVALUE></FEATURE></PRODUCT_FEATURES>" +
                             "<PRODUCT_FEATURES><FEATURE><FNAME>FARBE</FNAME><FVALUE>blau</FVALUE></FEATURE></PRODUCT_FEATURES>" +
                             "</PRODUCT>";

            Product p = _Load(_Document2005(product)).Products.Single();
            p.RemoveFeature("farbe");

            Assert.IsNull(p.GetFeature("farbe"));
            Assert.IsNotNull(p.GetFeature("gewicht"));
        } // !RemoveFeatureRemovesTheFeatureFromAllFeatureSets()
        #endregion // !Features


        #region Catalog structure
        [TestMethod]
        public void CatalogStructureAndProductMappingsAreRead_2005()
        {
            string content = "<CATALOG_GROUP_SYSTEM>" +
                             "<CATALOG_STRUCTURE type=\"node\"><GROUP_ID>1</GROUP_ID><GROUP_NAME>Büro</GROUP_NAME><PARENT_ID>0</PARENT_ID></CATALOG_STRUCTURE>" +
                             "<CATALOG_STRUCTURE type=\"leaf\"><GROUP_ID>11</GROUP_ID><GROUP_NAME>Stifte</GROUP_NAME><PARENT_ID>1</PARENT_ID></CATALOG_STRUCTURE>" +
                             "</CATALOG_GROUP_SYSTEM>" +
                             "<PRODUCT mode=\"new\"><SUPPLIER_PID>P1</SUPPLIER_PID></PRODUCT>" +
                             "<PRODUCT mode=\"new\"><SUPPLIER_PID>P2</SUPPLIER_PID></PRODUCT>" +
                             "<PRODUCT_TO_CATALOGGROUP_MAP><PROD_ID>P1</PROD_ID><CATALOG_GROUP_ID>11</CATALOG_GROUP_ID><PRODUCT_TO_CATALOGGROUP_MAP_ORDER>3</PRODUCT_TO_CATALOGGROUP_MAP_ORDER></PRODUCT_TO_CATALOGGROUP_MAP>";

            ProductCatalog catalog = _Load(_Document2005(content));

            Assert.AreEqual(2, catalog.CatalogStructures.Count);
            CatalogStructure leaf = catalog.CatalogStructures.Single(s => s.GroupId == "11");
            Assert.AreEqual(CatalogStructureTypes.Leaf, leaf.Type);
            Assert.AreEqual("Stifte", leaf.GroupName);
            Assert.AreEqual("1", leaf.ParentId);

            ProductCatalogGroupMapping mapping = catalog.Products.Single(p => p.No == "P1").ProductCatalogGroupMappings.Single();
            Assert.AreEqual("11", mapping.CatalogGroupId);
            Assert.AreEqual(3, mapping.Order);
            Assert.AreEqual(0, catalog.Products.Single(p => p.No == "P2").ProductCatalogGroupMappings.Count);
        } // !CatalogStructureAndProductMappingsAreRead_2005()


        [TestMethod]
        public void ArticleMappingsAreRead_12()
        {
            string content = "<ARTICLE mode=\"new\"><SUPPLIER_AID>A1</SUPPLIER_AID></ARTICLE>" +
                             "<ARTICLE_TO_CATALOGGROUP_MAP><ART_ID>A1</ART_ID><CATALOG_GROUP_ID>11</CATALOG_GROUP_ID></ARTICLE_TO_CATALOGGROUP_MAP>" +
                             "<ARTICLE_TO_CATALOGGROUP_MAP><ART_ID>A1</ART_ID><CATALOG_GROUP_ID>12</CATALOG_GROUP_ID></ARTICLE_TO_CATALOGGROUP_MAP>";

            ProductCatalog catalog = _Load(_Document12(content));

            CollectionAssert.AreEquivalent(new List<string>() { "11", "12" }, catalog.Products.Single().ProductCatalogGroupMappings.Select(m => m.CatalogGroupId).ToList());
        } // !ArticleMappingsAreRead_12()
        #endregion // !Catalog structure


        #region Helpers
        private ProductCatalog _Load(string xml)
        {
            return ProductCatalog.Load(new MemoryStream(Encoding.UTF8.GetBytes(xml)));
        } // !_Load()


        private string _Document2005(string newCatalogContent)
        {
            return "<?xml version=\"1.0\" encoding=\"utf-8\"?>" +
                   $"<BMECAT version=\"2005\" xmlns=\"{Namespace2005}\">" +
                   "<HEADER><CATALOG><LANGUAGE>deu</LANGUAGE><CATALOG_ID>CAT</CATALOG_ID><CATALOG_VERSION>1.0</CATALOG_VERSION></CATALOG></HEADER>" +
                   $"<T_NEW_CATALOG>{newCatalogContent}</T_NEW_CATALOG>" +
                   "</BMECAT>";
        } // !_Document2005()


        private string _Document12(string newCatalogContent)
        {
            return "<?xml version=\"1.0\" encoding=\"utf-8\"?>" +
                   $"<BMECAT version=\"1.2\" xmlns=\"{Namespace12}\">" +
                   "<HEADER><CATALOG><LANGUAGE>deu</LANGUAGE><CATALOG_ID>CAT</CATALOG_ID><CATALOG_VERSION>1.0</CATALOG_VERSION></CATALOG></HEADER>" +
                   $"<T_NEW_CATALOG>{newCatalogContent}</T_NEW_CATALOG>" +
                   "</BMECAT>";
        } // !_Document12()
        #endregion // !Helpers
    }
}
