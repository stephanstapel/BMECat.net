using BMECat.net;
using System.Text.Json;

namespace BMECat.net_Test
{
    [TestClass]
    public class BasicTests
    {
        [TestMethod]
        public void SimpleTest()
        {
            ProductCatalog catalog = _GenerateSimpleCatalog();
            catalog.Save("test.xml");
        }
        

        [TestMethod]
        public async Task SynchronousAsynchronousComparisonTest()
        {
            ProductCatalog catalog = _GenerateSimpleCatalog();
            MemoryStream ms = new MemoryStream();
            catalog.Save(ms);


            ms.Position = 0;
            ProductCatalog synchronouslyLoadedCatalog = ProductCatalog.Load(ms);            
            string synchronouslyLoadedCatalogJson = JsonSerializer.Serialize(synchronouslyLoadedCatalog);

            ms.Position = 0;
            ProductCatalog asynchronouslyLoadedCatalog = await ProductCatalog.LoadAsync(ms);            
            string asynchronouslyLoadedCatalogJson = JsonSerializer.Serialize(synchronouslyLoadedCatalog);

            Assert.AreEqual(synchronouslyLoadedCatalogJson, asynchronouslyLoadedCatalogJson);

            Task.WaitAll();
        } // !SynchronousAsynchronousTest()


        private ProductCatalog _GenerateSimpleCatalog()
        {
            ProductCatalog catalog = new ProductCatalog()
            {
                Languages = { LanguageCodes.DEU },
                CatalogId = "QA_CAT_002",
                CatalogVersion = "001.002",
                CatalogName = "Office Material",
                GenerationDate = new System.DateTime(2004, 8, 20, 10, 59, 54),
                Currency = CurrencyCodes.EUR
            };

            catalog.Products.Add(new Product()
            {
                No = "Q20-P09",
                EANCode = "0000000011",
                DescriptionShort = "Post-Safe Polythene Envelopes Deutsch",
                DescriptionLong = "Deutsch All-weather lightweight envelopes protect your contents and save you money. ALL - WEATHER.Once sealed, Post-Safe envelopes are completely waterproof.Your contents won't get damaged.",
                Stock = 100,
                Prices = new List<ProductPrice>()
                {
                    new ProductPrice()
                    {
                        Currency = CurrencyCodes.EUR,
                        Amount = 16.49m,
                        Tax = 0.19m
                    }
                }
            });

            return catalog;
        } // !_GenerateSimpleCatalog()
    }
}