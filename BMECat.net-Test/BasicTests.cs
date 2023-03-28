using BMECat.net;
using System.Text;
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
        public async Task SynchronousAsynchronousLoadingTest()
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
        } // !SynchronousAsynchronousLoadingTest()


        [TestMethod]
        public async Task SynchronousAsynchronousSavingTest()
        {
            ProductCatalog catalog = _GenerateSimpleCatalog();


            MemoryStream msSynchronous = new MemoryStream();
            catalog.Save(msSynchronous);
            string  synchronousProductCatalog = System.Text.Encoding.UTF8.GetString(msSynchronous.ToArray());

            MemoryStream msAsynchronous = new MemoryStream();
            await catalog.SaveAsync(msAsynchronous);
            string asynchronousProductCatalog = System.Text.Encoding.UTF8.GetString(msAsynchronous.ToArray());
            
            Assert.AreEqual(synchronousProductCatalog, asynchronousProductCatalog);

            Task.WaitAll();
        } // !SynchronousAsynchronousLoadingTest()


        [TestMethod]
        public async Task RoundtripProductIdHandling_NoProductId()
        {
            ProductCatalog catalog = _GenerateSimpleCatalog();

            catalog.Products.First().PIds.Clear();

            MemoryStream ms = new MemoryStream();
            catalog.Save(ms);

            ms.Position = 0;
            ProductCatalog loadedCatalog = await ProductCatalog.LoadAsync(ms);
            Assert.AreEqual(loadedCatalog.Products.First().PIds.Count, 0);

            Task.WaitAll();
        }


        [TestMethod]
        public async Task RoundtripProductIdHandling_GTIN_only()
        {
            string _id = System.Guid.NewGuid().ToString();

            ProductCatalog catalog = _GenerateSimpleCatalog();

            catalog.Products.First().PIds.Clear();
            catalog.Products.First().PIds.Add(new ProductId() { Type = ProductIdTypes.GTIN, Id = _id });

            MemoryStream ms = new MemoryStream();
            catalog.Save(ms);
            catalog.Save("e:\\output.xml");

            ms.Position = 0;
            ProductCatalog loadedCatalog = await ProductCatalog.LoadAsync(ms);
            Assert.AreEqual(loadedCatalog.Products.First().PIds.Count, 1);
            Assert.AreEqual(loadedCatalog.Products.First().PIds[0].Type, ProductIdTypes.GTIN);
            Assert.AreEqual(loadedCatalog.Products.First().PIds[0].Id, _id);

            Task.WaitAll();
        }


        [TestMethod]
        public async Task RoundtripProductIdHandling_EAN_only()
        {
            string _id = System.Guid.NewGuid().ToString();

            ProductCatalog catalog = _GenerateSimpleCatalog();

            catalog.Products.First().PIds.Clear();
            catalog.Products.First().PIds.Add(new ProductId() { Type = ProductIdTypes.EAN, Id = _id });

            MemoryStream ms = new MemoryStream();
            catalog.Save(ms);
            catalog.Save("e:\\output.xml");

            ms.Position = 0;
            ProductCatalog loadedCatalog = await ProductCatalog.LoadAsync(ms);
            Assert.AreEqual(loadedCatalog.Products.First().PIds.Count, 1);
            Assert.AreEqual(loadedCatalog.Products.First().PIds[0].Type, ProductIdTypes.EAN);
            Assert.AreEqual(loadedCatalog.Products.First().PIds[0].Id, _id);

            Task.WaitAll();
        }


        [TestMethod]
        public async Task RoundtripProductIdHandling_supplier_specific_only()
        {
            string _id = System.Guid.NewGuid().ToString();

            ProductCatalog catalog = _GenerateSimpleCatalog();

            catalog.Products.First().PIds.Clear();
            catalog.Products.First().PIds.Add(new ProductId() { Type = ProductIdTypes.SupplierSpecific, Id = _id });

            MemoryStream ms = new MemoryStream();
            catalog.Save(ms);

            ms.Position = 0;
            ProductCatalog loadedCatalog = await ProductCatalog.LoadAsync(ms);
            Assert.AreEqual(loadedCatalog.Products.First().PIds.Count, 1);
            Assert.AreEqual(loadedCatalog.Products.First().PIds[0].Type, ProductIdTypes.SupplierSpecific);
            Assert.AreEqual(loadedCatalog.Products.First().PIds[0].Id, _id);

            Task.WaitAll();
        }


        [TestMethod]
        public async Task RoundtripProductIdHandling_EAN_GTIN()
        {
            string _id0 = System.Guid.NewGuid().ToString();
            string _id1 = System.Guid.NewGuid().ToString();

            ProductCatalog catalog = _GenerateSimpleCatalog();

            catalog.Products.First().PIds.Clear();
            catalog.Products.First().PIds.Add(new ProductId() { Type = ProductIdTypes.EAN, Id = _id0 });
            catalog.Products.First().PIds.Add(new ProductId() { Type = ProductIdTypes.GTIN, Id = _id1 });

            MemoryStream ms = new MemoryStream();
            catalog.Save(ms);

            ms.Position = 0;
            ProductCatalog loadedCatalog = await ProductCatalog.LoadAsync(ms);
            Assert.AreEqual(loadedCatalog.Products.First().PIds.Count, 2);
            Assert.AreEqual(loadedCatalog.Products.First().PIds[0].Type, ProductIdTypes.EAN);
            Assert.AreEqual(loadedCatalog.Products.First().PIds[0].Id, _id0);
            Assert.AreEqual(loadedCatalog.Products.First().PIds[1].Type, ProductIdTypes.GTIN);
            Assert.AreEqual(loadedCatalog.Products.First().PIds[1].Id, _id1);

            Task.WaitAll();
        }


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
                PIds = new List<ProductId>() { new ProductId() { Type = ProductIdTypes.EAN, Id = "0000000011" } },
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