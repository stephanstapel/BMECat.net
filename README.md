# BMECat.net
[![NuGet](https://img.shields.io/nuget/v/BMECat.net?color=blue)](https://www.nuget.org/packages/BMECat.net/)
BMECat.net is a .net open source library that allows you to read and write BMEcat descriptions would be a software component that provides developers with a set of tools and functionalities to manipulate BMEcat files programmatically. This library would enable software applications to easily integrate BMEcat support, allowing for the efficient exchange of product information between different systems and partners. Developers could use this library to parse, create, and modify BMEcat documents, accessing product data in a standardized and structured way. Overall, this library simplifies the development of software applications that rely on BMEcat for product information exchange.

The library supports both version 1.2 and 2005. Support for extended data structures (like product features, address details) is for reading only so far. If you need to create BMECat files with extended data structures, drop me a message.

The library has a sister library (https://github.com/stephanstapel/OpenTrans.net) which allows to process order files.


# License
Subject to the Apache license http://www.apache.org/licenses/LICENSE-2.0.html

# Installation
Just use nuget or Visual Studio Package Manager and download 'BMECat.net'.

You can find more information about the nuget package here:

[![NuGet](https://img.shields.io/nuget/v/BMECat.net?color=blue)](https://www.nuget.org/packages/BMECat.net/)

https://www.nuget.org/packages/BMECat.net/

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
