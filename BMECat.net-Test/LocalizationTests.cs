using BMECat.net;
using System.Globalization;
using System.Text;

namespace BMECat.net_Test
{
    /// <summary>
    /// Reading and writing BMECat documents has to work independently from the culture
    /// settings of the machine that executes the code.
    ///
    /// BMECat is an xml based format, hence all numbers and dates have to be written
    /// using the invariant (xml) notation, e.g. 0.19 instead of 0,19 and
    /// 2004-08-20T14:59:54 instead of 20.08.2004 02:59:54.
    /// </summary>
    [TestClass]
    public class LocalizationTests
    {
        /// <summary>
        /// Comma as decimal separator, dot as group separator
        /// </summary>
        private static readonly CultureInfo GERMAN = new CultureInfo("de-DE");

        /// <summary>
        /// Dot as decimal separator, comma as group separator
        /// </summary>
        private static readonly CultureInfo AMERICAN = new CultureInfo("en-US");

        /// <summary>
        /// Buddhist calendar, i.e. the year 2004 is written as 2547
        /// </summary>
        private static readonly CultureInfo THAI = new CultureInfo("th-TH");

        /// <summary>
        /// Dotless lower case i, i.e. GTIN.ToLower() results in gtın instead of gtin
        /// </summary>
        private static readonly CultureInfo TURKISH = new CultureInfo("tr-TR");


        #region Writing numbers
        [TestMethod]
        public void TaxIsWrittenWithDecimalPoint_GermanCulture()
        {
            string xml = _RunWithCulture(GERMAN, () => _WriteCatalog(_GenerateCatalog()));

            StringAssert.Contains(xml, "<TAX>0.19</TAX>");
            Assert.IsFalse(xml.Contains("<TAX>0,19</TAX>"), "TAX must not be written using the culture specific decimal separator");
        } // !TaxIsWrittenWithDecimalPoint_GermanCulture()


        [TestMethod]
        public void PriceAmountIsWrittenWithDecimalPoint_GermanCulture()
        {
            string xml = _RunWithCulture(GERMAN, () => _WriteCatalog(_GenerateCatalog()));

            StringAssert.Contains(xml, "<PRICE_AMOUNT>16.49</PRICE_AMOUNT>");
            Assert.IsFalse(xml.Contains("<PRICE_AMOUNT>16,49</PRICE_AMOUNT>"), "PRICE_AMOUNT must not be written using the culture specific decimal separator");
        } // !PriceAmountIsWrittenWithDecimalPoint_GermanCulture()


        [TestMethod]
        public void IntegerElementsAreWrittenWithoutGroupSeparator_GermanCulture()
        {
            ProductCatalog catalog = _GenerateCatalog();
            catalog.Products.First().Stock = 100000;
            catalog.Products.First().Prices.First().LowerBound = 1000;

            string xml = _RunWithCulture(GERMAN, () => _WriteCatalog(catalog));

            StringAssert.Contains(xml, "<STOCK>100000</STOCK>");
            StringAssert.Contains(xml, "<LOWER_BOUND>1000</LOWER_BOUND>");
        } // !IntegerElementsAreWrittenWithoutGroupSeparator_GermanCulture()


        [TestMethod]
        public void LowerBoundIsOmittedWhenNotSet()
        {
            ProductCatalog catalog = _GenerateCatalog();
            catalog.Products.First().Prices.First().LowerBound = null;

            string xml = _RunWithCulture(GERMAN, () => _WriteCatalog(catalog));

            Assert.IsFalse(xml.Contains("LOWER_BOUND"), "An unset lower bound must not be written at all");

            ProductCatalog loadedCatalog = _RunWithCulture(GERMAN, () => _ReadCatalog(xml));
            Assert.IsNull(loadedCatalog.Products.First().Prices.First().LowerBound, "An omitted lower bound has to be read back as null");
        } // !LowerBoundIsOmittedWhenNotSet()


        [TestMethod]
        public void WrittenDocumentIsIdenticalAcrossCultures()
        {
            ProductCatalog catalog = _GenerateCatalog();

            string germanXml = _RunWithCulture(GERMAN, () => _WriteCatalog(catalog));
            string americanXml = _RunWithCulture(AMERICAN, () => _WriteCatalog(catalog));
            string thaiXml = _RunWithCulture(THAI, () => _WriteCatalog(catalog));

            Assert.AreEqual(germanXml, americanXml);
            Assert.AreEqual(germanXml, thaiXml);
        } // !WrittenDocumentIsIdenticalAcrossCultures()
        #endregion // !Writing numbers


        #region Writing dates
        [TestMethod]
        public void GenerationDateIsWrittenAsTwentyFourHourTime()
        {
            ProductCatalog catalog = _GenerateCatalog();
            catalog.GenerationDate = new DateTime(2004, 8, 20, 14, 59, 54);

            string xml = _RunWithCulture(AMERICAN, () => _WriteCatalog(catalog));

            StringAssert.Contains(xml, "2004-08-20T14:59:54", "The afternoon has to be written as 14:59:54, not as 02:59:54");
        } // !GenerationDateIsWrittenAsTwentyFourHourTime()


        [TestMethod]
        public void GenerationDateIsWrittenUsingGregorianCalendar_ThaiCulture()
        {
            ProductCatalog catalog = _GenerateCatalog();
            catalog.GenerationDate = new DateTime(2004, 8, 20, 10, 59, 54);

            string xml = _RunWithCulture(THAI, () => _WriteCatalog(catalog));

            StringAssert.Contains(xml, "2004-08-20", "The year has to be written using the gregorian calendar, not as buddhist year 2547");
        } // !GenerationDateIsWrittenUsingGregorianCalendar_ThaiCulture()
        #endregion // !Writing dates


        #region Reading numbers
        [TestMethod]
        public void CommaSeparatedTaxIsNotReadAsGroupSeparator()
        {
            // simulates a catalog that was written by a non conformant, localized application
            string xml = _WriteCatalog(_GenerateCatalog()).Replace("<TAX>0.19</TAX>", "<TAX>0,19</TAX>");

            ProductCatalog catalog = _ReadCatalog(xml);
            decimal tax = catalog.Products.First().Prices.First().Tax;

            Assert.AreNotEqual(19m, tax, "0,19 must not be interpreted as the group separated value 019");
            Assert.AreEqual(0.19m, tax);
        } // !CommaSeparatedTaxIsNotReadAsGroupSeparator()


        [TestMethod]
        public void CommaSeparatedPriceAmountIsNotReadAsGroupSeparator()
        {
            // simulates a catalog that was written by a non conformant, localized application
            string xml = _WriteCatalog(_GenerateCatalog()).Replace("<PRICE_AMOUNT>16.49</PRICE_AMOUNT>", "<PRICE_AMOUNT>16,49</PRICE_AMOUNT>");

            ProductCatalog catalog = _ReadCatalog(xml);
            decimal amount = catalog.Products.First().Prices.First().Amount;

            Assert.AreNotEqual(1649m, amount, "16,49 must not be interpreted as the group separated value 1649");
            Assert.AreEqual(16.49m, amount);
        } // !CommaSeparatedPriceAmountIsNotReadAsGroupSeparator()


        [TestMethod]
        public void AmbiguousDecimalCommaIsNotGuessed()
        {
            // 1,234 is either a decimal comma or an english group separator, hence the value has to be rejected
            string xml = _WriteCatalog(_GenerateCatalog()).Replace("<PRICE_AMOUNT>16.49</PRICE_AMOUNT>", "<PRICE_AMOUNT>1,234</PRICE_AMOUNT>");

            ProductCatalog catalog = _ReadCatalog(xml);

            Assert.AreEqual(0m, catalog.Products.First().Prices.First().Amount, "An ambiguous value has to fall back to the default value");
        } // !AmbiguousDecimalCommaIsNotGuessed()


        [TestMethod]
        public void PricesAreReadIdenticallyAcrossCultures()
        {
            string xml = _WriteCatalog(_GenerateCatalog());

            ProductPrice germanPrice = _RunWithCulture(GERMAN, () => _ReadCatalog(xml).Products.First().Prices.First());
            ProductPrice americanPrice = _RunWithCulture(AMERICAN, () => _ReadCatalog(xml).Products.First().Prices.First());
            ProductPrice thaiPrice = _RunWithCulture(THAI, () => _ReadCatalog(xml).Products.First().Prices.First());

            Assert.AreEqual(16.49m, germanPrice.Amount);
            Assert.AreEqual(0.19m, germanPrice.Tax);

            Assert.AreEqual(germanPrice.Amount, americanPrice.Amount);
            Assert.AreEqual(germanPrice.Tax, americanPrice.Tax);

            Assert.AreEqual(germanPrice.Amount, thaiPrice.Amount);
            Assert.AreEqual(germanPrice.Tax, thaiPrice.Tax);
        } // !PricesAreReadIdenticallyAcrossCultures()


        [TestMethod]
        public void StockIsReadIdenticallyAcrossCultures()
        {
            ProductCatalog catalog = _GenerateCatalog();
            catalog.Products.First().Stock = 100000;
            string xml = _WriteCatalog(catalog);

            int? germanStock = _RunWithCulture(GERMAN, () => _ReadCatalog(xml).Products.First().Stock);
            int? americanStock = _RunWithCulture(AMERICAN, () => _ReadCatalog(xml).Products.First().Stock);

            Assert.AreEqual(100000, germanStock);
            Assert.AreEqual(germanStock, americanStock);
        } // !StockIsReadIdenticallyAcrossCultures()
        #endregion // !Reading numbers


        #region Reading enumerations
        [TestMethod]
        public void ProductIdTypeIsReadIdenticallyAcrossCultures()
        {
            ProductCatalog catalog = _GenerateCatalog();
            catalog.Products.First().PIds.Clear();
            catalog.Products.First().PIds.Add(new ProductId() { Type = ProductIdTypes.GTIN, Id = "0000000011" });

            // upper case type attribute, as it is used by several catalog exports
            string xml = _WriteCatalog(catalog).Replace("type=\"gtin\"", "type=\"GTIN\"");

            ProductIdTypes turkishType = _RunWithCulture(TURKISH, () => _ReadCatalog(xml).Products.First().PIds.First().Type);
            ProductIdTypes americanType = _RunWithCulture(AMERICAN, () => _ReadCatalog(xml).Products.First().PIds.First().Type);

            Assert.AreEqual(ProductIdTypes.GTIN, americanType);
            Assert.AreEqual(ProductIdTypes.GTIN, turkishType, "GTIN must not become gtın when lowercasing in the turkish culture");
        } // !ProductIdTypeIsReadIdenticallyAcrossCultures()


        [TestMethod]
        public void PriceTypeIsReadIdenticallyAcrossCultures()
        {
            // upper case price type attribute, as it is used by several catalog exports
            string xml = _WriteCatalog(_GenerateCatalog()).Replace("price_type=\"net_list\"", "price_type=\"NET_LIST\"");

            PriceTypes? turkishType = _RunWithCulture(TURKISH, () => _ReadCatalog(xml).Products.First().Prices.First().PriceType);
            PriceTypes? americanType = _RunWithCulture(AMERICAN, () => _ReadCatalog(xml).Products.First().Prices.First().PriceType);

            Assert.AreEqual(PriceTypes.NetList, americanType);
            Assert.AreEqual(PriceTypes.NetList, turkishType, "NET_LIST must not become net_lıst when lowercasing in the turkish culture");
        } // !PriceTypeIsReadIdenticallyAcrossCultures()
        #endregion // !Reading enumerations


        #region Reading dates
        [TestMethod]
        public void GenerationDateIsReadIdenticallyAcrossCultures()
        {
            ProductCatalog catalog = _GenerateCatalog();
            catalog.GenerationDate = new DateTime(2004, 8, 20, 10, 59, 54);
            string xml = _WriteCatalog(catalog);

            DateTime? germanDate = _RunWithCulture(GERMAN, () => _ReadCatalog(xml).GenerationDate);
            DateTime? americanDate = _RunWithCulture(AMERICAN, () => _ReadCatalog(xml).GenerationDate);
            DateTime? thaiDate = _RunWithCulture(THAI, () => _ReadCatalog(xml).GenerationDate);

            Assert.AreEqual(2004, germanDate!.Value.Year);
            Assert.AreEqual(germanDate, americanDate);
            Assert.AreEqual(germanDate, thaiDate, "The buddhist calendar must not be applied when parsing xml dates");
        } // !GenerationDateIsReadIdenticallyAcrossCultures()
        #endregion // !Reading dates


        #region Roundtrips
        [TestMethod]
        public void RoundtripPricesAreCultureIndependent()
        {
            ProductCatalog catalog = _GenerateCatalog();

            // written on a german machine, read on an american machine
            string xml = _RunWithCulture(GERMAN, () => _WriteCatalog(catalog));
            ProductPrice price = _RunWithCulture(AMERICAN, () => _ReadCatalog(xml).Products.First().Prices.First());

            Assert.AreEqual(16.49m, price.Amount);
            Assert.AreEqual(0.19m, price.Tax);
        } // !RoundtripPricesAreCultureIndependent()


        [TestMethod]
        public void RoundtripGenerationDateIsCultureIndependent()
        {
            ProductCatalog catalog = _GenerateCatalog();
            catalog.GenerationDate = new DateTime(2004, 8, 20, 14, 59, 54);

            string xml = _RunWithCulture(THAI, () => _WriteCatalog(catalog));
            DateTime? generationDate = _RunWithCulture(THAI, () => _ReadCatalog(xml).GenerationDate);

            Assert.AreEqual(catalog.GenerationDate, generationDate);
        } // !RoundtripGenerationDateIsCultureIndependent()
        #endregion // !Roundtrips


        #region Helpers
        private T _RunWithCulture<T>(CultureInfo culture, Func<T> function)
        {
            CultureInfo previousCulture = CultureInfo.CurrentCulture;
            CultureInfo previousUICulture = CultureInfo.CurrentUICulture;

            try
            {
                CultureInfo.CurrentCulture = culture;
                CultureInfo.CurrentUICulture = culture;
                return function();
            }
            finally
            {
                CultureInfo.CurrentCulture = previousCulture;
                CultureInfo.CurrentUICulture = previousUICulture;
            }
        } // !_RunWithCulture()


        private string _WriteCatalog(ProductCatalog catalog)
        {
            MemoryStream ms = new MemoryStream();
            catalog.Save(ms);
            return Encoding.UTF8.GetString(ms.ToArray());
        } // !_WriteCatalog()


        private ProductCatalog _ReadCatalog(string xml)
        {
            MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(xml));
            ms.Position = 0;
            return ProductCatalog.Load(ms);
        } // !_ReadCatalog()


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
