using BMECat.net;
using System.Globalization;
using System.Text;
using System.Xml;

namespace BMECat.net_Test
{
    /// <summary>
    /// Everything the writer writes has to be read back unchanged
    /// </summary>
    [TestClass]
    public class WriterRoundtripTests
    {
        private const string Namespace20051 = "http://www.bmecat.org/bmecat/2005.1";


        #region Parties
        [TestMethod]
        public void BuyerAndSupplierSurviveRoundtrip()
        {
            ProductCatalog loaded = _Roundtrip(_GenerateCatalog());

            Party supplier = loaded.Supplier;
            Assert.IsNotNull(supplier);
            Assert.AreEqual("SUP-1", supplier.Id);
            Assert.AreEqual("Lieferant GmbH", supplier.Name);
            Assert.AreEqual("Vertrieb", supplier.Name2);
            Assert.AreEqual("Haus B", supplier.Name3);
            Assert.AreEqual("Innendienst", supplier.Department);
            Assert.AreEqual("Hauptstr. 1", supplier.Street);
            Assert.AreEqual("12345", supplier.Zip);
            Assert.AreEqual("Berlin", supplier.City);
            Assert.AreEqual("DE", supplier.Country);
            Assert.AreEqual("DE123456789", supplier.VATID);
            Assert.AreEqual("+49 30 123456", supplier.Phone);

            Party buyer = loaded.Buyer;
            Assert.IsNotNull(buyer);
            Assert.AreEqual("BUY-1", buyer.Id);
            Assert.AreEqual("Kunde AG", buyer.Name);
            Assert.AreEqual("Hamburg", buyer.City);
        } // !BuyerAndSupplierSurviveRoundtrip()


        [TestMethod]
        public void ContactDetailsSurviveRoundtrip()
        {
            ContactDetails contact = _Roundtrip(_GenerateCatalog()).Supplier.ContactDetails.Single();

            Assert.AreEqual("Mustermann", contact.Name);
            Assert.AreEqual("Erika", contact.FirstName);
            Assert.AreEqual("Dr.", contact.AcademicTitle);
            CollectionAssert.AreEqual(new List<string>() { "+49 30 1", "+49 30 2" }, contact.Phone);
            Assert.AreEqual("erika.mustermann@example.com", contact.Emails);
        } // !ContactDetailsSurviveRoundtrip()


        [TestMethod]
        public void PartiesAreReferencedFromTheHeader()
        {
            XmlDocument doc = _WriteToDocument(_GenerateCatalog(), out XmlNamespaceManager nsmgr);

            Assert.AreEqual("SUP-1", doc.SelectSingleNode("/bmecat:BMECAT/bmecat:HEADER/bmecat:SUPPLIER_IDREF", nsmgr)?.InnerText);
            Assert.AreEqual("BUY-1", doc.SelectSingleNode("/bmecat:BMECAT/bmecat:HEADER/bmecat:BUYER_IDREF", nsmgr)?.InnerText);
            Assert.AreEqual(2, doc.SelectNodes("/bmecat:BMECAT/bmecat:HEADER/bmecat:PARTIES/bmecat:PARTY", nsmgr)!.Count);
            Assert.IsNull(doc.SelectSingleNode("/bmecat:BMECAT/bmecat:HEADER/bmecat:BUYER", nsmgr), "The deprecated BUYER element must not be written anymore");
        } // !PartiesAreReferencedFromTheHeader()


        [TestMethod]
        public void CatalogWithoutPartiesDoesNotWritePartiesElement()
        {
            ProductCatalog catalog = _GenerateCatalog();
            catalog.Buyer = null;
            catalog.Supplier = null;

            XmlDocument doc = _WriteToDocument(catalog, out XmlNamespaceManager nsmgr);

            Assert.IsNull(doc.SelectSingleNode("//bmecat:PARTIES", nsmgr));
            Assert.IsNull(doc.SelectSingleNode("//bmecat:SUPPLIER_IDREF", nsmgr));
        } // !CatalogWithoutPartiesDoesNotWritePartiesElement()
        #endregion // !Parties


        #region Product content
        [TestMethod]
        public void FeaturesAndClassificationSurviveRoundtrip()
        {
            Product p = _Roundtrip(_GenerateCatalog()).Products[0];

            FeatureSet featureSet = p.FeatureSets.Single();
            Assert.AreEqual("ETIM-9.0", featureSet.FeatureClassificationSystem.Classification);
            Assert.AreEqual("EC000001", featureSet.FeatureClassificationSystem.GroupIds.Single().Name);

            Assert.AreEqual(2, featureSet.Features.Count);
            Feature length = p.GetFeature("EF000008")!;
            Assert.AreEqual("12.5", length.Values.Single());
            Assert.AreEqual(QuantityCodes.MMT, length.Unit.Code);
            Assert.AreEqual("1", length.Order);
            Assert.AreEqual("Länge", length.Description);

            CollectionAssert.AreEqual(new List<string>() { "EV000001", "EV000002" }, p.GetFeature("EF000020")!.Values);
        } // !FeaturesAndClassificationSurviveRoundtrip()


        [TestMethod]
        public void KeywordsSurviveRoundtrip()
        {
            Product p = _Roundtrip(_GenerateCatalog()).Products[0];

            CollectionAssert.AreEqual(new List<string>() { "Schraube", "Mutter" }, p.Keywords);
        } // !KeywordsSurviveRoundtrip()


        [TestMethod]
        public void MimeInfosSurviveRoundtrip()
        {
            List<MimeInfo> mimeInfos = _Roundtrip(_GenerateCatalog()).Products[0].MimeInfos;

            Assert.AreEqual(2, mimeInfos.Count);
            Assert.AreEqual(MimeTypes.ImageJpeg, mimeInfos[0].MimeType);
            Assert.AreEqual("images/p1.jpg", mimeInfos[0].Source);
            Assert.AreEqual("Produktbild", mimeInfos[0].Description);
            Assert.AreEqual("Schraube M4", mimeInfos[0].Alt);
            Assert.AreEqual("normal", mimeInfos[0].Purpose);
            Assert.AreEqual(1, mimeInfos[0].Order);
            Assert.AreEqual(MimeTypes.ApplicationPdf, mimeInfos[1].MimeType);
            Assert.AreEqual("docs/p1.pdf", mimeInfos[1].Source);
        } // !MimeInfosSurviveRoundtrip()


        [TestMethod]
        public void ReferencesSurviveRoundtrip()
        {
            List<Reference> references = _Roundtrip(_GenerateCatalog()).Products[0].References;

            Assert.AreEqual(2, references.Count);
            Assert.AreEqual(ReferenceTypes.Accessories, references[0].Type);
            Assert.AreEqual("P2", references[0].IdTo);
            Assert.AreEqual(ReferenceTypes.SparePart, references[1].Type);
            Assert.AreEqual("P3", references[1].IdTo);
        } // !ReferencesSurviveRoundtrip()


        [TestMethod]
        public void LogisticsDetailsSurviveRoundtrip_GermanCulture()
        {
            ProductCatalog catalog = _GenerateCatalog();

            // decimal comma culture, the dimensions have to be written in xml notation nevertheless
            CultureInfo previousCulture = CultureInfo.CurrentCulture;
            CultureInfo.CurrentCulture = new CultureInfo("de-DE");
            ProductCatalog loaded;
            try
            {
                loaded = _Roundtrip(catalog);
            }
            finally
            {
                CultureInfo.CurrentCulture = previousCulture;
            }

            LogisticsDetails logistics = loaded.Products[0].LogisticsDetails;
            Assert.IsNotNull(logistics);
            Assert.AreEqual(CountryCodes.DE, logistics.CountryOfOrigin);
            CollectionAssert.AreEqual(new List<string>() { "73181595" }, logistics.CustomsTariffNumber);
            Assert.AreEqual(0.0125m, logistics.Weight);
            Assert.AreEqual(40m, logistics.Length);
            Assert.AreEqual(8.5m, logistics.Width);
            Assert.IsNull(logistics.Depth);
            Assert.IsNull(logistics.Volume);
        } // !LogisticsDetailsSurviveRoundtrip_GermanCulture()


        [TestMethod]
        public void ProductWithoutOptionalStructuresDoesNotWriteEmptyElements()
        {
            ProductCatalog catalog = _GenerateCatalog();
            catalog.Products.Add(new Product() { No = "P-EMPTY", DescriptionShort = "leer" });

            XmlDocument doc = _WriteToDocument(catalog, out XmlNamespaceManager nsmgr);
            XmlNode product = doc.SelectSingleNode("//bmecat:PRODUCT[bmecat:SUPPLIER_PID='P-EMPTY']", nsmgr)!;

            Assert.IsNotNull(product);
            Assert.IsNull(product.SelectSingleNode("./bmecat:PRODUCT_FEATURES", nsmgr));
            Assert.IsNull(product.SelectSingleNode("./bmecat:MIME_INFO", nsmgr));
            Assert.IsNull(product.SelectSingleNode("./bmecat:PRODUCT_REFERENCE", nsmgr));
            Assert.IsNull(product.SelectSingleNode("./bmecat:PRODUCT_LOGISTIC_DETAILS", nsmgr));
            Assert.IsNull(doc.SelectSingleNode("//bmecat:PRODUCT_TO_CATALOGGROUP_MAP[bmecat:PROD_ID='P-EMPTY']", nsmgr));
        } // !ProductWithoutOptionalStructuresDoesNotWriteEmptyElements()
        #endregion // !Product content


        #region Catalog structure
        [TestMethod]
        public void CatalogStructureSurvivesRoundtrip()
        {
            List<CatalogStructure> structures = _Roundtrip(_GenerateCatalog()).CatalogStructures;

            Assert.AreEqual(2, structures.Count);
            Assert.AreEqual(CatalogStructureTypes.Node, structures[0].Type);
            Assert.AreEqual("1", structures[0].GroupId);
            Assert.AreEqual("Büro", structures[0].GroupName);
            Assert.AreEqual("0", structures[0].ParentId);

            Assert.AreEqual(CatalogStructureTypes.Leaf, structures[1].Type);
            Assert.AreEqual("11", structures[1].GroupId);
            Assert.AreEqual("1", structures[1].ParentId);
            Assert.AreEqual("2", structures[1].GroupOrder);
            Assert.AreEqual("groups/stifte.jpg", structures[1].MimeInfos.Single().Source);
        } // !CatalogStructureSurvivesRoundtrip()


        [TestMethod]
        public void CatalogGroupMappingsSurviveRoundtrip()
        {
            List<ProductCatalogGroupMapping> mappings = _Roundtrip(_GenerateCatalog()).Products[0].ProductCatalogGroupMappings;

            Assert.AreEqual(2, mappings.Count);
            Assert.AreEqual("11", mappings[0].CatalogGroupId);
            Assert.AreEqual(3, mappings[0].Order);
            Assert.AreEqual("1", mappings[1].CatalogGroupId);
            Assert.IsNull(mappings[1].Order);
        } // !CatalogGroupMappingsSurviveRoundtrip()
        #endregion // !Catalog structure


        #region Complete document
        [TestMethod]
        public void WritingAReadDocumentAgainProducesTheSameDocument()
        {
            string firstDocument = _Write(_GenerateCatalog());
            ProductCatalog loaded = ProductCatalog.Load(new MemoryStream(Encoding.UTF8.GetBytes(firstDocument)));
            string secondDocument = _Write(loaded);

            Assert.AreEqual(firstDocument, secondDocument);
        } // !WritingAReadDocumentAgainProducesTheSameDocument()


        [TestMethod]
        public void WrittenDocumentConformsToTheSchema()
        {
            // the writer uses the namespace of the 2005 final draft, the schema in the repository is the 2005.1 release
            string xml = _Write(_GenerateCatalog()).TrimStart('﻿').Replace("http://www.bmecat.org/bmecat/2005fd", Namespace20051)
                                                   .Replace("version=\"2005\"", "version=\"2005.1\"");

            XmlReaderSettings settings = new XmlReaderSettings() { ValidationType = ValidationType.Schema };
            settings.Schemas.Add(Namespace20051, _FindSchema());
            List<string> errors = new List<string>();
            settings.ValidationEventHandler += (sender, e) => errors.Add($"line {e.Exception.LineNumber}: {e.Message}");

            using (XmlReader reader = XmlReader.Create(new StringReader(xml), settings))
            {
                while (reader.Read())
                {
                }
            }

            Assert.AreEqual(0, errors.Count, String.Join(Environment.NewLine, errors));
        } // !WrittenDocumentConformsToTheSchema()
        #endregion // !Complete document


        #region Helpers
        private string _FindSchema()
        {
            // the tests run from bin/<configuration>/<framework>, the schema lives in the repository
            DirectoryInfo? directory = new DirectoryInfo(AppContext.BaseDirectory);
            while (directory != null)
            {
                string candidate = Path.Combine(directory.FullName, "Documentation and Samples", "bmecat_2005_1.xsd");
                if (File.Exists(candidate))
                {
                    return candidate;
                }
                directory = directory.Parent;
            }

            Assert.Inconclusive("bmecat_2005_1.xsd not found");
            return "";
        } // !_FindSchema()


        private ProductCatalog _Roundtrip(ProductCatalog catalog)
        {
            MemoryStream ms = new MemoryStream();
            catalog.Save(ms);
            return ProductCatalog.Load(ms);
        } // !_Roundtrip()


        private string _Write(ProductCatalog catalog)
        {
            MemoryStream ms = new MemoryStream();
            catalog.Save(ms);
            return Encoding.UTF8.GetString(ms.ToArray());
        } // !_Write()


        private XmlDocument _WriteToDocument(ProductCatalog catalog, out XmlNamespaceManager nsmgr)
        {
            MemoryStream ms = new MemoryStream();
            catalog.Save(ms);

            XmlDocument doc = new XmlDocument();
            doc.Load(ms);
            nsmgr = new XmlNamespaceManager(doc.NameTable);
            nsmgr.AddNamespace("bmecat", doc.DocumentElement!.NamespaceURI);
            return doc;
        } // !_WriteToDocument()


        private ProductCatalog _GenerateCatalog()
        {
            ProductCatalog catalog = new ProductCatalog()
            {
                Languages = { LanguageCodes.DEU },
                CatalogId = "QA_CAT_004",
                CatalogVersion = "001.004",
                CatalogName = "Komplettkatalog",
                GenerationDate = new DateTime(2004, 8, 20, 10, 59, 54),
                Currency = CurrencyCodes.EUR,
                Supplier = new Party()
                {
                    Id = "SUP-1",
                    Name = "Lieferant GmbH",
                    Name2 = "Vertrieb",
                    Name3 = "Haus B",
                    Department = "Innendienst",
                    Street = "Hauptstr. 1",
                    Zip = "12345",
                    City = "Berlin",
                    Country = "DE",
                    VATID = "DE123456789",
                    Phone = "+49 30 123456",
                    ContactDetails = new List<ContactDetails>()
                    {
                        new ContactDetails()
                        {
                            Name = "Mustermann",
                            FirstName = "Erika",
                            AcademicTitle = "Dr.",
                            Phone = new List<string>() { "+49 30 1", "+49 30 2" },
                            Emails = "erika.mustermann@example.com"
                        }
                    }
                },
                Buyer = new Party() { Id = "BUY-1", Name = "Kunde AG", City = "Hamburg" }
            };

            catalog.CatalogStructures.Add(new CatalogStructure() { Type = CatalogStructureTypes.Node, GroupId = "1", GroupName = "Büro", ParentId = "0" });
            catalog.CatalogStructures.Add(new CatalogStructure()
            {
                Type = CatalogStructureTypes.Leaf,
                GroupId = "11",
                GroupName = "Stifte",
                ParentId = "1",
                GroupOrder = "2",
                MimeInfos = { new MimeInfo() { MimeType = MimeTypes.ImageJpeg, Source = "groups/stifte.jpg" } }
            });

            FeatureSet featureSet = new FeatureSet()
            {
                FeatureClassificationSystem = new FeatureClassificationSystem()
                {
                    Classification = "ETIM-9.0",
                    GroupIds = { new FeatureClassificationSystemGroupId() { Name = "EC000001" } }
                }
            };
            featureSet.Features.Add(new Feature() { Name = "EF000008", Values = { "12.5" }, Unit = new QuantityCode() { ClearText = "MMT" }, Order = "1", Description = "Länge" });
            featureSet.Features.Add(new Feature() { Name = "EF000020", Values = { "EV000001", "EV000002" } });

            catalog.Products.Add(new Product()
            {
                No = "P1",
                DescriptionShort = "Schraube M4",
                Keywords = { "Schraube", "Mutter" },
                FeatureSets = { featureSet },
                OrderDetails = new OrderDetails() { OrderUnit = new QuantityCode() { ClearText = "C62" }, ContentUnit = new QuantityCode() { ClearText = "C62" } },
                Prices = { new ProductPrice() { Currency = CurrencyCodes.EUR, PriceType = PriceTypes.NetList, Amount = 9.95m, Tax = 0.19m } },
                MimeInfos =
                {
                    new MimeInfo() { MimeType = MimeTypes.ImageJpeg, Source = "images/p1.jpg", Description = "Produktbild", Alt = "Schraube M4", Purpose = "normal", Order = 1 },
                    new MimeInfo() { MimeType = MimeTypes.ApplicationPdf, Source = "docs/p1.pdf" }
                },
                References =
                {
                    new Reference() { Type = ReferenceTypes.Accessories, IdTo = "P2" },
                    new Reference() { Type = ReferenceTypes.SparePart, IdTo = "P3" }
                },
                LogisticsDetails = new LogisticsDetails()
                {
                    CountryOfOrigin = CountryCodes.DE,
                    CustomsTariffNumber = { "73181595" },
                    Weight = 0.0125m,
                    Length = 40m,
                    Width = 8.5m
                },
                ProductCatalogGroupMappings =
                {
                    new ProductCatalogGroupMapping() { CatalogGroupId = "11", Order = 3 },
                    new ProductCatalogGroupMapping() { CatalogGroupId = "1" }
                }
            });

            return catalog;
        } // !_GenerateCatalog()
        #endregion // !Helpers
    }
}
