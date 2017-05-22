/*
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at
 * 
 *   http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace BMECat.net
{
    internal class BMECatReader
    {
        public static ProductCatalog Load(Stream stream)
        {
            if (!stream.CanRead)
            {
                throw new IllegalStreamException("Cannot read from stream");
            }

            XmlDocument doc = new XmlDocument();
            doc.Load(stream);
            XmlNamespaceManager nsmgr = new XmlNamespaceManager(doc.DocumentElement.OwnerDocument.NameTable);
            nsmgr.AddNamespace("xsi", "http://www.bmecat.org/bmecat/1.2/bmecat_new_catalog");

            string version = XmlUtils.nodeAsString(doc.DocumentElement, "/BMECAT/@version");
            if (version != "2005")
            {
                throw new Exception("Only version 2005 is currently supported");
            }

            ProductCatalog retval = new ProductCatalog();

            foreach(XmlNode languageNode in doc.DocumentElement.SelectNodes("/BMECAT/HEADER/CATALOG/LANGUAGE"))
            {
                LanguageCodes language = default(LanguageCodes).FromString(languageNode.InnerText);
                retval.Languages.Add(language);
            }

            retval.CatalogId = XmlUtils.nodeAsString(doc.DocumentElement, "/BMECAT/HEADER/CATALOG/CATALOG_ID");
            retval.CatalogVersion = XmlUtils.nodeAsString(doc.DocumentElement, "/BMECAT/HEADER/CATALOG/CATALOG_VERSION");
            retval.CatalogName = XmlUtils.nodeAsString(doc.DocumentElement, "/BMECAT/HEADER/CATALOG/CATALOG_NAME");
            retval.GenerationDate = XmlUtils.nodeAsDateTime(doc.DocumentElement, "/BMECAT/HEADER/CATALOG/GENERATION_DATE");
            retval.Currency = default(CurrencyCodes).FromString(XmlUtils.nodeAsString(doc.DocumentElement, "/BMECAT/HEADER/CATALOG/CURRENCY"));

            XmlNodeList productNodes = doc.DocumentElement.SelectNodes("/BMECAT/T_NEW_CATALOG/PRODUCT");
            Parallel.ForEach(productNodes.Cast<XmlNode>(),
                             (XmlNode productNode) =>
            {
                string _productMode = XmlUtils.nodeAsString(productNode, "@mode");
                Product product = new Product()
                {
                    No = XmlUtils.nodeAsString(productNode, "./SUPPLIER_PID"),
                    DescriptionShort = XmlUtils.nodeAsString(productNode, "./PRODUCT_DETAILS/DESCRIPTION_SHORT"),
                    DescriptionLong = XmlUtils.nodeAsString(productNode, "./PRODUCT_DETAILS/DESCRIPTION_LONG"),
                    EANCode = XmlUtils.nodeAsString(productNode, "./PRODUCT_DETAILS/EAN"),
                    Stock = XmlUtils.nodeAsInt(productNode, "./PRODUCT_DETAILS/STOCK"),
                    OrderUnit = default(QuantityCodes).FromString(XmlUtils.nodeAsString(productNode, "./PRODUCT_ORDER_DETAILS/ORDER_UNIT")),
                    ContentUnit = default(QuantityCodes).FromString(XmlUtils.nodeAsString(productNode, "./PRODUCT_ORDER_DETAILS/CONTENT_UNIT")),
                    Currency = default(CurrencyCodes).FromString(XmlUtils.nodeAsString(productNode, "./PRODUCT_PRICE_DETAILS/PRODUCT_PRICE/PRICE_CURRENCY")),
                    VAT = XmlUtils.nodeAsInt(productNode, "./PRODUCT_PRICE_DETAILS/PRODUCT_PRICE/TAX")
                };

                decimal? _netPrice = XmlUtils.nodeAsDecimal(productNode, "./PRODUCT_PRICE_DETAILS/PRODUCT_PRICE/PRICE_AMOUNT");
                if (_netPrice.HasValue)
                {
                    product.NetPrice = _netPrice.Value;
                }

                retval.Products.Add(product);
            });

            return retval;
        } // !Load()


        public static ProductCatalog Load(string filename)
        {
            if (!System.IO.File.Exists(filename))
            {
                throw new FileNotFoundException();
            }

            return Load(new FileStream(filename, FileMode.Open, FileAccess.Read));
        } // !Load()
    }
}
