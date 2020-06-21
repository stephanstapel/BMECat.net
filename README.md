# BMECat.net
.net library for reading and writing BMECat product catalogues.

The library has a sister library (https://github.com/stephanstapel/OpenTrans.net) which allows to process order files.


# Installation
Installation so far is manually, since no nuget packages was created yet.

# Usage

Creation of catalogue files is simple:
```C#
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
  Currency = CurrencyCodes.EUR,
  NetPrice = 16.49m,
  DescriptionShort = "Post-Safe Polythene Envelopes Deutsch",
  DescriptionLong = "Deutsch All-weather lightweight envelopes protect your contents and save you money. ALL - WEATHER.Once sealed, Post-Safe envelopes are completely waterproof.Your contents won't get damaged.",
  Stock = 100,
  VAT = 19
});


catalog.Save("test.xml");
```

Create a new ProductCatalog object and set the necessary properties to set up the catalogue:

```C#
ProductCatalog catalog = new ProductCatalog()
{
  ...
}
```

Then add products to the product list property:

```C#
catalog.Products.Add(new Product()
{
  ...
}
```

and finally save the catalogue either to stream or to file:


```C#
catalog.Save("test.xml");
```

That's it.

# More information
More information about BMEcat can be found here:
* https://de.wikipedia.org/wiki/BMEcat
* http://www.bme.de/initiativen/bmecat/
