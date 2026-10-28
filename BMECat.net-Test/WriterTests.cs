using BMECat.net;
using System.Text;
using System.Xml;

namespace BMECat.net_Test
{
    /// <summary>
    /// Tests for writing BMECat 2005 documents and reading them back
    /// </summary>
    [TestClass]
    public class WriterTests
    {
        #region Stream handling
        [TestMethod]
        public void StreamIsResetToTheOriginalPositionAfterWriting()
        {
            MemoryStream ms = new MemoryStream();
            byte[] prefix = Encoding.UTF8.GetBytes("prefix");
            ms.Write(prefix, 0, prefix.Length);

            _GenerateCatalog().Save(ms);

            Assert.AreEqual(prefix.Length, ms.Position);
            Assert.IsTrue(ms.Length > prefix.Length);
        } // !StreamIsResetToTheOriginalPositionAfterWriting()


        [TestMethod]
        public async Task WritingToANonSeekableStreamFailsWithIllegalStreamException()
        {
            await Assert.ThrowsExactlyAsync<IllegalStreamException>(() => _GenerateCatalog().SaveAsync(new NonSeekableStream()));
        } // !WritingToANonSeekableStreamFailsWithIllegalStreamException()
        #endregion // !Stream handling


        #region Content
        [TestMethod]
        public void SpecialCharactersAreEscapedAndSurviveRoundtrip()
        {
            ProductCatalog catalog = _GenerateCatalog();
            catalog.CatalogName = "Müller & Söhne <Sortiment>";
            catalog.Products[0].DescriptionShort = "Schrauben \"M4\" & Muttern < 5 mm";

            ProductCatalog loaded = _Roundtrip(catalog);

            Assert.AreEqual(catalog.CatalogName, loaded.CatalogName);
            Assert.AreEqual(catalog.Products[0].DescriptionShort, loaded.Products[0].DescriptionShort);
        } // !SpecialCharactersAreEscapedAndSurviveRoundtrip()


        [TestMethod]
        public void HeaderSurvivesRoundtrip()
        {
            ProductCatalog catalog = _GenerateCatalog();
            catalog.Buyer = new Party() { Id = "BUY-1", Name = "Kunde AG" };

            ProductCatalog loaded = _Roundtrip(catalog);

            Assert.AreEqual(catalog.CatalogId, loaded.CatalogId);
            Assert.AreEqual(catalog.CatalogVersion, loaded.CatalogVersion);
            Assert.AreEqual(catalog.Currency, loaded.Currency);
            CollectionAssert.AreEqual(catalog.Languages, loaded.Languages);
            Assert.AreEqual("BUY-1", loaded.Buyer.Id);
            Assert.AreEqual("Kunde AG", loaded.Buyer.Name);
        } // !HeaderSurvivesRoundtrip()


        [TestMethod]
        public void ScaledPricesSurviveRoundtrip()
        {
            ProductCatalog catalog = _GenerateCatalog();
            catalog.Products[0].Prices.Add(new ProductPrice() { Currency = CurrencyCodes.EUR, PriceType = PriceTypes.NetList, Amount = 14.99m, Tax = 0.19m, LowerBound = 10 });

            List<ProductPrice> prices = _Roundtrip(catalog).Products[0].Prices;

            Assert.AreEqual(2, prices.Count);
            Assert.AreEqual(16.49m, prices[0].Amount);
            Assert.IsNull(prices[0].LowerBound);
            Assert.AreEqual(14.99m, prices[1].Amount);
            Assert.AreEqual(10, prices[1].LowerBound);
            Assert.AreEqual(CurrencyCodes.EUR, prices[1].Currency);
        } // !ScaledPricesSurviveRoundtrip()


        [TestMethod]
        public void OrderUnitsSurviveRoundtrip()
        {
            ProductCatalog catalog = _GenerateCatalog();
            catalog.Products[0].OrderDetails = new OrderDetails()
            {
                OrderUnit = new QuantityCode() { ClearText = "C62" },
                ContentUnit = new QuantityCode() { ClearText = "C62" }
            };

            OrderDetails orderDetails = _Roundtrip(catalog).Products[0].OrderDetails;

            Assert.AreEqual(QuantityCodes.C62, orderDetails.OrderUnit.Code);
            Assert.AreEqual(QuantityCodes.C62, orderDetails.ContentUnit.Code);
        } // !OrderUnitsSurviveRoundtrip()


        [TestMethod]
        public void OrderQuantitiesSurviveRoundtrip()
        {
            ProductCatalog catalog = _GenerateCatalog();
            catalog.Products[0].OrderDetails = new OrderDetails()
            {
                OrderUnit = new QuantityCode() { ClearText = "C62" },
                ContentUnit = new QuantityCode() { ClearText = "C62" },
                ContentUnitPerOrderUnit = 100,
                PriceQuantity = 1,
                QuantityMin = 2,
                QuantityInterval = 2
            };

            OrderDetails orderDetails = _Roundtrip(catalog).Products[0].OrderDetails;

            Assert.AreEqual(100, orderDetails.ContentUnitPerOrderUnit);
            Assert.AreEqual(1, orderDetails.PriceQuantity);
            Assert.AreEqual(2, orderDetails.QuantityMin);
            Assert.AreEqual(2, orderDetails.QuantityInterval);
        } // !OrderQuantitiesSurviveRoundtrip()


        [TestMethod]
        public void UnsetOrderUnitsAreNotWrittenAsEmptyElements()
        {
            ProductCatalog catalog = _GenerateCatalog();
            catalog.Products[0].OrderDetails = new OrderDetails();

            XmlDocument doc = _WriteToDocument(catalog, out XmlNamespaceManager nsmgr);

            Assert.AreEqual(0, doc.SelectNodes("//bmecat:ORDER_UNIT", nsmgr)!.Count);
            Assert.AreEqual(0, doc.SelectNodes("//bmecat:CONTENT_UNIT", nsmgr)!.Count);
        } // !UnsetOrderUnitsAreNotWrittenAsEmptyElements()


        [TestMethod]
        public void QuantityCodeConverterIsUsedForWriting()
        {
            ProductCatalog catalog = _GenerateCatalog();
            catalog.Products[0].OrderDetails = new OrderDetails()
            {
                OrderUnit = new QuantityCode() { Code = QuantityCodes.C62 }
            };
            BMECatExtensions extensions = new BMECatExtensions() { QuantityCodeConverter = new FixedQuantityCodeConverter("Stück") };

            MemoryStream ms = new MemoryStream();
            catalog.Save(ms, extensions);
            XmlDocument doc = _LoadDocument(ms, out XmlNamespaceManager nsmgr);

            Assert.AreEqual("Stück", doc.SelectSingleNode("//bmecat:ORDER_UNIT", nsmgr)?.InnerText);
        } // !QuantityCodeConverterIsUsedForWriting()


        [TestMethod]
        public void QuantityCodeIsWrittenWhenTheConverterDoesNotKnowIt()
        {
            ProductCatalog catalog = _GenerateCatalog();
            catalog.Products[0].OrderDetails = new OrderDetails()
            {
                OrderUnit = new QuantityCode() { Code = QuantityCodes.C62 }
            };
            BMECatExtensions extensions = new BMECatExtensions() { QuantityCodeConverter = new FixedQuantityCodeConverter(null!) };

            MemoryStream ms = new MemoryStream();
            catalog.Save(ms, extensions);
            XmlDocument doc = _LoadDocument(ms, out XmlNamespaceManager nsmgr);

            Assert.AreEqual("C62", doc.SelectSingleNode("//bmecat:ORDER_UNIT", nsmgr)?.InnerText);
        } // !QuantityCodeIsWrittenWhenTheConverterDoesNotKnowIt()


        [TestMethod]
        public void MissingOrderUnitIsReportedAndQuantitiesAreStillWritten()
        {
            ProductCatalog catalog = _GenerateCatalog();
            catalog.Products[0].OrderDetails = new OrderDetails() { QuantityMin = 2 };
            RecordingLogService logService = new RecordingLogService();

            MemoryStream ms = new MemoryStream();
            catalog.Save(ms, new BMECatExtensions() { LogService = logService });
            XmlDocument doc = _LoadDocument(ms, out XmlNamespaceManager nsmgr);

            Assert.AreEqual(1, logService.Warnings.Count);
            StringAssert.Contains(logService.Warnings[0], catalog.Products[0].No);
            Assert.AreEqual("2", doc.SelectSingleNode("//bmecat:QUANTITY_MIN", nsmgr)?.InnerText);
        } // !MissingOrderUnitIsReportedAndQuantitiesAreStillWritten()


        [TestMethod]
        public void UnsetOptionalProductElementsAreNotWritten()
        {
            ProductCatalog catalog = _GenerateCatalog();
            catalog.Products[0].Stock = null;

            XmlDocument doc = _WriteToDocument(catalog, out XmlNamespaceManager nsmgr);

            Assert.IsNull(doc.SelectSingleNode("//bmecat:STOCK", nsmgr));
            Assert.IsNull(doc.SelectSingleNode("//bmecat:MANUFACTURER_PID", nsmgr));
            Assert.IsNull(doc.SelectSingleNode("//bmecat:MANUFACTURER_NAME", nsmgr));
            Assert.IsNull(doc.SelectSingleNode("//bmecat:DESCRIPTION_LONG", nsmgr));
        } // !UnsetOptionalProductElementsAreNotWritten()
        #endregion // !Content


        #region Helpers
        private ProductCatalog _Roundtrip(ProductCatalog catalog)
        {
            MemoryStream ms = new MemoryStream();
            catalog.Save(ms);
            return ProductCatalog.Load(ms);
        } // !_Roundtrip()


        private XmlDocument _WriteToDocument(ProductCatalog catalog, out XmlNamespaceManager nsmgr)
        {
            MemoryStream ms = new MemoryStream();
            catalog.Save(ms);
            return _LoadDocument(ms, out nsmgr);
        } // !_WriteToDocument()


        private XmlDocument _LoadDocument(MemoryStream ms, out XmlNamespaceManager nsmgr)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(ms);

            nsmgr = new XmlNamespaceManager(doc.NameTable);
            nsmgr.AddNamespace("bmecat", doc.DocumentElement!.NamespaceURI);
            return doc;
        } // !_LoadDocument()


        private ProductCatalog _GenerateCatalog()
        {
            ProductCatalog catalog = new ProductCatalog()
            {
                Languages = { LanguageCodes.DEU },
                CatalogId = "QA_CAT_003",
                CatalogVersion = "001.003",
                CatalogName = "Office Material",
                GenerationDate = new DateTime(2004, 8, 20, 10, 59, 54),
                Currency = CurrencyCodes.EUR,
                Supplier = new Party() { Id = "SUP-1", Name = "Lieferant GmbH" }
            };

            catalog.Products.Add(new Product()
            {
                No = "Q20-P09",
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
