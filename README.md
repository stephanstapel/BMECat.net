# BMECat.net
[![NuGet](https://img.shields.io/nuget/v/BMECat.net?color=blue)](https://www.nuget.org/packages/BMECat.net/)

BMECat.net is a .net open source library that allows you to read and write BMEcat descriptions would be a software component that provides developers with a set of tools and functionalities to manipulate BMEcat files programmatically. This library would enable software applications to easily integrate BMEcat support, allowing for the efficient exchange of product information between different systems and partners. Our aim is to support real world BMECat files which often do not match the standard 100%.
Developers could use this library to parse, create, and modify BMEcat documents, accessing product data in a standardized and structured way. Overall, this library simplifies the development of software applications that rely on BMEcat for product information exchange.

The library supports both version 1.2 and 2005 (including 2005fd). Reading of extended data structures (like product features, address details) is implemented.
If you are looking for writing special elements and extended data structures, I'm happy to accept sponsoring.

Core features:
* High performance: reads a 10K product catalog with all extensions in under 4 seconds on an average i7 workstation
* Trading time for memory: focus on execution time instead of memory consumption.
* Version agnostic: allows to read catalog structures without paying attention to the actual version
* Non-standard-friendly: supports various non-standard elements, mainly though the mix of 1.2 and 2005 formats that are found in real life

The library has a sister library (https://github.com/stephanstapel/OpenTrans.net) which allows to process order files.


# License
Subject to the Apache license http://www.apache.org/licenses/LICENSE-2.0.html

# Installation
Just use nuget or Visual Studio Package Manager and download 'BMECat.net'.

You can find more information about the nuget package here:

[![NuGet](https://img.shields.io/nuget/v/BMECat.net?color=blue)](https://www.nuget.org/packages/BMECat.net/)

https://www.nuget.org/packages/BMECat.net/

# Usage

Loading existing catalogue file is possible in both synchronous and asynchronous way. Synchronously, it works like this:

```C#
ProductCatalog catalog = ProductCatalog.Load("test.xml");
```

while asynchronously, loading works like this:


```C#
ProductCatalog catalog = await ProductCatalog.LoadAsync("test.xml");
```

The library automatically detects if the BMECat file is written in 1.2 format or 2005 standard or a mixture (see below for details about non-standard formats).

If you should want to create a product catalog based on your own data and export it into BMECat, use this code:

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

and finally save the catalogue either to stream or to file. This is possible in synchronous and asynchronous way. Synchronously works like this:


```C#
catalog.Save("test.xml");
```

and asynchronously, it works like this:


```C#
await catalog.SaveAsync("test.xml");
```

The standard export format is BMECat 2005. If you should want to export legacy BMECat 1.2 as well, please drop me a message or issue a pull request. I'm more than happy to provide feedback and merge it.


# Real world BMECat files

BMECat in the wild is not always what the designers thought it should be. The library currently supports:

* Missing xml namespace information on <BMECAT> root node (BMECat 2005)
* Using of buyer and supplier elements instead of parties (BMECat 2005)
* Handling of different quantity codes via BMECatExtension class (all versions)

# Building on your own

Prerequisites:
* Visual Studio >= 2017
* .net Framework >= 4.6.1 (for .net Standard 2.0 support)

Open BMECat.net/BMECat.net.sln solution file. Choose Release or Debug mode and hit 'Build'. That's it.

For running the tests, open BMECat.net-Test/BMECat.net-Test.sln and run the unit tests. The tests show good cases on how to use the library.

# More information
More information about BMEcat can be found here:
* https://de.wikipedia.org/wiki/BMEcat
* http://www.bme.de/initiativen/bmecat/
