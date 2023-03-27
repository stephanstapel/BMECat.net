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
using BMECat.net.ETIM;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace BMECat.net
{
    internal class BMECatReader2005 : BMECatReaderBase
    {
        internal static ProductCatalog Load(Stream inputStream, BMECatExtensions extensions = null)
        {
            if (inputStream == null)
            {
                return null;
            }

            // make sure that the root element contains xmlns elements to indicate which namespace to use
            MemoryStream convertedStream = new MemoryStream();
            byte[] firstPartOfBuffer = new byte[1024];
            inputStream.Read(firstPartOfBuffer, 0, 1024);
            string firstPartString = System.Text.Encoding.UTF8.GetString(firstPartOfBuffer);

            if (!firstPartString.Contains("xmlns"))
            {
                string pattern = "<BMECAT.*?>";
                string replacement = @"<BMECAT xmlns=""http://www.bmecat.org/bmecat/2005"" version=""2005"">";
                string outputString = Regex.Replace(firstPartString, pattern, replacement);
                byte[] outputBuffer = System.Text.Encoding.UTF8.GetBytes(outputString);                
                convertedStream.Write(outputBuffer, 0, outputBuffer.Length);
            }
            else
            {
                inputStream.Position = 0;
            }

            inputStream.CopyTo(convertedStream);
            convertedStream.Position = 0;

            XmlDocument doc = new XmlDocument();
            doc.Load(convertedStream);

            XmlNamespaceManager nsmgr = new XmlNamespaceManager(doc.NameTable);            

            string xmlnsURL = XmlUtils.AttributeText(doc.DocumentElement, "xmlns");
            if (!String.IsNullOrEmpty(xmlnsURL))
            {
                nsmgr.AddNamespace("bmecat", xmlnsURL);
            }
            
            ProductCatalog retval = new ProductCatalog();


            foreach(XmlNode languageNode in XmlUtils.SelectNodes(doc, "/bmecat:BMECAT/bmecat:HEADER/bmecat:CATALOG/bmecat:LANGUAGE", nsmgr))
            {
                LanguageCodes language = default(LanguageCodes).FromString(languageNode.InnerText);
                retval.Languages.Add(language); 
            }

            retval.CatalogId = XmlUtils.nodeAsString(doc, "/bmecat:BMECAT/bmecat:HEADER/bmecat:CATALOG/bmecat:CATALOG_ID", nsmgr);
            retval.CatalogVersion = XmlUtils.nodeAsString(doc, "/bmecat:BMECAT/bmecat:HEADER/bmecat:CATALOG/bmecat:CATALOG_VERSION", nsmgr);
            retval.CatalogName = XmlUtils.nodeAsString(doc.DocumentElement, "/bmecat:BMECAT/bmecat:HEADER/CATALOGbmecat:/bmecat:CATALOG_NAME", nsmgr);
            retval.GenerationDate = XmlUtils.nodeAsDateTime(doc.DocumentElement, "/bmecat:BMECAT/bmecat:HEADER/bmecat:CATALOG/bmecat:GENERATION_DATE", nsmgr);
            retval.Currency = default(CurrencyCodes).FromString(XmlUtils.nodeAsString(doc.DocumentElement, "/bmecat:BMECAT/bmecat:HEADER/bmecat:CATALOG/bmecat:CURRENCY", nsmgr));

            XmlNode agreementNode = XmlUtils.SelectSingleNode(doc, "/bmecat:BMECAT/bmecat:HEADER/bmecat:AGREEMENT", nsmgr);
            if (agreementNode != null)
            {
                retval.Agreement = new Agreement()
                {
                    Id = XmlUtils.nodeAsString(agreementNode, "./bmecat:AGREEMENT_ID", nsmgr),
                    StartDate = XmlUtils.nodeAsDateTime(agreementNode, "./bmecat:AGREEMENT_START_DATE", nsmgr),
                    EndDate = XmlUtils.nodeAsDateTime(agreementNode, "./bmecat:AGREEMENT_END_DATE", nsmgr),
                };
            }

            XmlNode partiesNode = XmlUtils.SelectSingleNode(doc, "/bmecat:BMECAT/bmecat:HEADER/bmecat:PARTIES", nsmgr);
            if (partiesNode != null)
            {
                foreach (XmlNode partyNode in XmlUtils.SelectNodes(partiesNode, "./bmecat:PARTY", nsmgr))
                {
                    Party party = null;
                    XmlNode addressNode = XmlUtils.SelectSingleNode(doc, "./bmecat:ADDRESS", nsmgr);
                    if (addressNode != null)
                    {
                        party = _ReadPartyAddress(addressNode, nsmgr);
                    }
                    else
                    {
                        party = new Party();
                    }

                    /* @todo regard id types */
                    party.Id = XmlUtils.nodeAsString(partyNode, "./bmecat:PARTY_ID", nsmgr);
                    party.Name = XmlUtils.nodeAsString(partyNode, "./bmecat:ADDRESS/bmecat:NAME", nsmgr);

                    string partyRole = XmlUtils.nodeAsString(partyNode, "./bmecat:PARTY_ROLE", nsmgr);
                    switch (partyRole)
                    {
                        case "buyer": retval.Buyer = party; break;
                        case "supplier": retval.Supplier = party; break;
                        default: break;
                    }
                }

                // legacy 1. Some supplier use classic supplier* and buyer* structures inside party nodes
                if ((retval.Buyer == null) && (partiesNode.SelectSingleNode("./bmecat:PARTY/bmecat:BUYER_ID", nsmgr) != null))
                {
                    /* @todo regard id types */
                    retval.Buyer = new Party()
                    {
                        Id = XmlUtils.nodeAsString(partiesNode, "./bmecat:PARTY/bmecat:BUYER_ID", nsmgr),
                        Name = XmlUtils.nodeAsString(partiesNode, "./bmecat:PARTY/bmecat:BUYER_NAME", nsmgr),
                        ContactName = XmlUtils.nodeAsString(partiesNode, "./bmecat:PARTY/bmecat:BUYER_CONTACT", nsmgr),
                        Street = XmlUtils.nodeAsString(partiesNode, "./bmecat:PARTY/bmecat:BUYER_STREET", nsmgr),
                        Zip = XmlUtils.nodeAsString(partiesNode, "./bmecat:PARTY/bmecat:BUYER_ZIP", nsmgr),
                        City = XmlUtils.nodeAsString(partiesNode, "./bmecat:PARTY/bmecat:BUYER_CITY", nsmgr),
                        Country = XmlUtils.nodeAsString(partiesNode, "./bmecat:PARTY/bmecat:BUYER_COUNTRY", nsmgr),
                        Phone = XmlUtils.nodeAsString(partiesNode, "./bmecat:PARTY/bmecat:BUYER_PHONE", nsmgr)

                        /* @todo read BUYER_EMAIL, BUYER_URL, BUYER_REMARKS */
                    };
                }

                if ((retval.Supplier == null) && (partiesNode.SelectSingleNode("./bmecat:PARTY/bmecat:SUPPLIER_ID", nsmgr) != null))
                {
                    /* @todo regard id types */
                    retval.Supplier = new Party()
                    {
                        Id = XmlUtils.nodeAsString(partiesNode, "./bmecat:PARTY/bmecat:SUPPLIER_ID", nsmgr),
                        Name = XmlUtils.nodeAsString(partiesNode, "./bmecat:PARTY/bmecat:SUPPLIER_NAME", nsmgr),
                        ContactName = XmlUtils.nodeAsString(partiesNode, "./bmecat:PARTY/bmecat:SUPPLIER_CONTACT", nsmgr),
                        Street = XmlUtils.nodeAsString(partiesNode, "./bmecat:PARTY/bmecat:SUPPLIER_STREET", nsmgr),
                        Zip = XmlUtils.nodeAsString(partiesNode, "./bmecat:PARTY/bmecat:SUPPLIER_ZIP", nsmgr),
                        City = XmlUtils.nodeAsString(partiesNode, "./bmecat:PARTY/bmecat:SUPPLIER_CITY", nsmgr),
                        Country = XmlUtils.nodeAsString(partiesNode, "./bmecat:PARTY/bmecat:SUPPLIER_COUNTRY", nsmgr),
                        Phone = XmlUtils.nodeAsString(partiesNode, "./bmecat:PARTY/bmecat:SUPPLIER_PHONE", nsmgr),

                        /* @todo read SUPPLIER_EMAIL, SUPPLIER_URL, SUPPLIER_REMARKS */
                    };
                }
            }
            else
            {
                // legacy 2. For example, Siemens(tm) still uses the old buyer, supplier structures
                XmlNode buyerNode = XmlUtils.SelectSingleNode(doc, "/bmecat:BMECAT/bmecat:HEADER/bmecat:BUYER", nsmgr);
                if (buyerNode != null)
                {
                    XmlNode addressNode = XmlUtils.SelectSingleNode(doc, "./bmecat:ADDRESS", nsmgr);
                    if (addressNode != null)
                    {
                        retval.Buyer = _ReadPartyAddress(addressNode, nsmgr);
                    }
                    else
                    {
                        retval.Buyer = new Party();
                    }

                    /* @todo regard id types */
                    retval.Buyer.Id = XmlUtils.nodeAsString(buyerNode, "./bmecat:BUYER_ID", nsmgr);
                    retval.Buyer.Name = XmlUtils.nodeAsString(buyerNode, "./bmecat:BUYER_NAME", nsmgr);
                }

                XmlNode supplierNode = XmlUtils.SelectSingleNode(doc, "/bmecat:BMECAT/bmecat:HEADER/bmecat:SUPPLIER", nsmgr);
                if (supplierNode != null)
                {

                    XmlNode addressNode = XmlUtils.SelectSingleNode(doc, "./bmecat:ADDRESS", nsmgr);
                    if (addressNode != null)
                    {
                        retval.Supplier = _ReadPartyAddress(addressNode, nsmgr);
                    }
                    else
                    {
                        retval.Supplier = new Party();
                    }

                    /* @todo regard id types */
                    retval.Supplier.Id = XmlUtils.nodeAsString(supplierNode, "./bmecat:SUPPLIER_ID", nsmgr);
                    retval.Supplier.Name = XmlUtils.nodeAsString(supplierNode, "./bmecat:SUPPLIER_NAME", nsmgr);
                }
            }


            Mutex mutex = new Mutex();
            XmlNodeList productNodes = XmlUtils.SelectNodes(doc.DocumentElement, "/bmecat:BMECAT/bmecat:T_NEW_CATALOG/bmecat:PRODUCT", nsmgr);            
            Parallel.ForEach(productNodes.Cast<XmlNode>(), /* new ParallelOptions() {  MaxDegreeOfParallelism = 1 }, */
                             (XmlNode productNode) =>
            {
                Product product = _ReadProduct(productNode, nsmgr, extensions);
                mutex.WaitOne();
                retval.Products.Add(product);
                mutex.ReleaseMutex();
            });

            XmlNodeList catalogNodes = XmlUtils.SelectNodes(doc.DocumentElement, "/bmecat:BMECAT/bmecat:T_NEW_CATALOG/bmecat:CATALOG_GROUP_SYSTEM/bmecat:CATALOG_STRUCTURE", nsmgr);
            Parallel.ForEach(catalogNodes.Cast<XmlNode>(), /* new ParallelOptions() {  MaxDegreeOfParallelism = 1 }, */
                             (XmlNode catalogNode) =>
                             {
                                 CatalogStructure catalogStructure = _ReadCatalogStructure(catalogNode, nsmgr);
                                 mutex.WaitOne();
                                 retval.CatalogStructures.Add(catalogStructure);
                                 mutex.ReleaseMutex();
                             });

            return retval;
        } // !Load()


        private static CatalogStructure _ReadCatalogStructure(XmlNode catalogNode, XmlNamespaceManager nsmgr)
        {
            CatalogStructure catalogStructure = new CatalogStructure()
            {
                Type = default(CatalogStructureTypes).FromString(XmlUtils.AttributeText(catalogNode, "type")),
                GroupId = XmlUtils.nodeAsString(catalogNode, "./bmecat:GROUP_ID", nsmgr),
                GroupName = XmlUtils.nodeAsString(catalogNode, "./bmecat:GROUP_NAME", nsmgr),
                ParentId = XmlUtils.nodeAsString(catalogNode, "./bmecat:PARENT_ID", nsmgr),
                GroupOrder = XmlUtils.nodeAsString(catalogNode, "./bmecat:GROUP_ORDER", nsmgr),
            };

            foreach (XmlNode mimeNode in XmlUtils.SelectNodes(catalogNode, "./bmecat:MIME_INFO/bmecat:MIME", nsmgr))
            {
                catalogStructure.MimeInfos.Add(new MimeInfo()
                {
                    MimeType = default(MimeTypes).FromString(XmlUtils.nodeAsString(mimeNode, "./bmecat:MIME_TYPE", nsmgr)),
                    Source = XmlUtils.nodeAsString(mimeNode, "./bmecat:MIME_SOURCE", nsmgr),
                    Description = XmlUtils.nodeAsString(mimeNode, "./bmecat:MIME_DESCR", nsmgr),
                    Purpose = XmlUtils.nodeAsString(mimeNode, "./bmecat:MIME_PURPOSE", nsmgr),
                    Alt = XmlUtils.nodeAsString(mimeNode, "./bmecat:MIME_ALT", nsmgr),
                    Order = XmlUtils.nodeAsInt(mimeNode, "./bmecat:MIME_ORDER", nsmgr),
                });
            }

            return catalogStructure;
        } // !_ReadCatalogStructure()


        private static Product _ReadProduct(XmlNode productNode, XmlNamespaceManager nsmgr, BMECatExtensions extensions)
        {
            string _productMode = XmlUtils.nodeAsString(productNode, "@mode", nsmgr);
            Product product = new Product()
            {
                No = XmlUtils.nodeAsString(productNode, "./bmecat:SUPPLIER_PID", nsmgr),
                DescriptionShort = XmlUtils.nodeAsString(productNode, "./bmecat:PRODUCT_DETAILS/bmecat:DESCRIPTION_SHORT", nsmgr),
                DescriptionLong = XmlUtils.nodeAsString(productNode, "./bmecat:PRODUCT_DETAILS/bmecat:DESCRIPTION_LONG", nsmgr),
                EANCode = XmlUtils.nodeAsString(productNode, "./bmecat:PRODUCT_DETAILS/bmecat:EAN", nsmgr),
                Stock = XmlUtils.nodeAsInt(productNode, "./bmecat:PRODUCT_DETAILS/bmecat:STOCK", nsmgr),
                SupplierAltPid = XmlUtils.nodeAsString(productNode, "./ARTICLE_DETAILS/SUPPLIER_ALT_PID", nsmgr),
                ManufacturerPID = XmlUtils.nodeAsString(productNode, "./ARTICLE_DETAILS/MANUFACTURER_PID", nsmgr),
                ManufacturerName = XmlUtils.nodeAsString(productNode, "./ARTICLE_DETAILS/MANUFACTURER_NAME", nsmgr),
                ManufacturerTypeDescription = XmlUtils.nodeAsString(productNode, "./ARTICLE_DETAILS/MANUFACTURER_TYPE_DESCR", nsmgr),
                ERPGroupSupplier = XmlUtils.nodeAsString(productNode, "./ARTICLE_DETAILS/ERP_GROUP_SUPPLIER", nsmgr),
                ERPGroupBuyer = XmlUtils.nodeAsString(productNode, "./ARTICLE_DETAILS/ERP_GROUP_BUYER", nsmgr),
            };

            foreach (XmlNode supplierPIdNode in XmlUtils.SelectNodes(productNode, "./bmecat:SUPPLIER_PID", nsmgr))
            {
                product.SupplierPIds.Add(new SupplierProductId()
                {
                    Type = default(SupplierProductIdTypes).FromString(XmlUtils.AttributeText(supplierPIdNode, "type")),
                    Id = supplierPIdNode.InnerText
                });
            }

            foreach (XmlNode keywordNode in XmlUtils.SelectNodes(productNode, "./bmecat:PRODUCT_DETAILS/bmecat:KEYWORD", nsmgr))
            {
                product.Keywords.Add(keywordNode.InnerText);
            }

            foreach (XmlNode featureNode in XmlUtils.SelectNodes(productNode, "./bmecat:PRODUCT_FEATURES/bmecat:FEATURE", nsmgr))
            {
                product.ProductFeatures.Add(new Feature()
                {
                    Name = XmlUtils.nodeAsString(featureNode, "./bmecat:FNAME", nsmgr),
                    Values = XmlUtils.nodesAsStrings(featureNode, "./bmecat:FVALUE", nsmgr),
                    Unit = _convertQuantityCode(XmlUtils.nodeAsString(featureNode, "./bmecat:FUNIT", nsmgr, null), extensions),
                    Order = XmlUtils.nodeAsString(featureNode, "./bmecat:FORDER", nsmgr)
                    /* TODO: FVALUE_TYPE */
                });
            }

            XmlNode orderDetailsNode = XmlUtils.SelectSingleNode(productNode, "./bmecat:PRODUCT_ORDER_DETAILS", nsmgr);
            if (orderDetailsNode != null)
            {
                product.OrderDetails = new OrderDetails()
                {
                    OrderUnit = _convertQuantityCode(XmlUtils.nodeAsString(productNode, "./bmecat:PRODUCT_ORDER_DETAILS/bmecat:ORDER_UNIT", nsmgr, null), extensions),
                    ContentUnit = _convertQuantityCode(XmlUtils.nodeAsString(productNode, "./bmecat:PRODUCT_ORDER_DETAILS/bmecat:CONTENT_UNIT", nsmgr, null), extensions),
                    ContentUnitPerOrderUnit = XmlUtils.nodeAsInt(orderDetailsNode, "./bmecat:NO_CU_PER_OU", nsmgr),
                    PriceQuantity = XmlUtils.nodeAsInt(orderDetailsNode, "./bmecat:PRICE_QUANTITY", nsmgr),
                    QuantityMin = XmlUtils.nodeAsInt(orderDetailsNode, "./bmecat:QUANTITY_MIN", nsmgr),
                    QuantityInterval = XmlUtils.nodeAsInt(orderDetailsNode, "./bmecat:QUANTITY_INTERVAL", nsmgr),
                };
            }

            foreach (XmlNode priceDetailNode in XmlUtils.SelectNodes(productNode, "./bmecat:PRODUCT_PRICE_DETAILS/bmecat:PRODUCT_PRICE", nsmgr))
            {
                product.Prices.Add(new ProductPrice()
                {
                    PriceType = default(PriceTypes).FromString(XmlUtils.AttributeText(priceDetailNode, "price_type")),
                    Amount = XmlUtils.nodeAsDecimal(priceDetailNode, "./bmecat:PRICE_AMOUNT", nsmgr, 0).Value,
                    Currency = default(CurrencyCodes).FromString(XmlUtils.nodeAsString(priceDetailNode, "./bmecat:PRICE_CURRENCY", nsmgr)),
                    Tax = XmlUtils.nodeAsDecimal(priceDetailNode, "./bmecat:TAX", nsmgr, 0).Value,
                    LowerBound = XmlUtils.nodeAsInt(priceDetailNode, "./bmecat:LOWER_BOUND", nsmgr)
                });
            }

            foreach (XmlNode mimeNode in XmlUtils.SelectNodes(productNode, "./bmecat:MIME_INFO/bmecat:MIME", nsmgr))
            {
                product.MimeInfos.Add(new MimeInfo()
                {
                    MimeType = default(MimeTypes).FromString(XmlUtils.nodeAsString(mimeNode, "./bmecat:MIME_TYPE", nsmgr)),
                    Source = XmlUtils.nodeAsString(mimeNode, "./bmecat:MIME_SOURCE", nsmgr),
                    Description = XmlUtils.nodeAsString(mimeNode, "./bmecat:MIME_DESCR", nsmgr),
                    Alt = XmlUtils.nodeAsString(mimeNode, "./bmecat:MIME_ALT", nsmgr),
                    Purpose = XmlUtils.nodeAsString(mimeNode, "./bmecat:MIME_PURPOSE", nsmgr),
                    Order = XmlUtils.nodeAsInt(mimeNode, "./bmecat:MIME_ORDER", nsmgr)
                });
            }

            XmlNode logisticDetailsNode = XmlUtils.SelectSingleNode(productNode, "./bmecat:PRODUCT_LOGISTIC_DETAILS", nsmgr);
            if (logisticDetailsNode != null)
            {
                product.LogisticsDetails = new LogisticsDetails()
                {
                    CountryOfOrigin = default(CountryCodes).FromString(XmlUtils.nodeAsString(logisticDetailsNode, "./bmecat:COUNTRY_OF_ORIGIN", nsmgr)),
                    CustomsTariffNumber = XmlUtils.nodesAsStrings(logisticDetailsNode, "./bmecat:CUSTOMS_TARIFF_NUMBER", nsmgr),
                    Volume = XmlUtils.nodeAsDecimal(logisticDetailsNode, "./bmecat:PRODUCT_DIMENSIONS/bmecat:VOLUME", nsmgr),
                    Weight = XmlUtils.nodeAsDecimal(logisticDetailsNode, "./bmecat:PRODUCT_DIMENSIONS/bmecat:WEIGHT", nsmgr),
                    Length = XmlUtils.nodeAsDecimal(logisticDetailsNode, "./bmecat:PRODUCT_DIMENSIONS/bmecat:LENGTH", nsmgr),
                    Width = XmlUtils.nodeAsDecimal(logisticDetailsNode, "./bmecat:PRODUCT_DIMENSIONS/bmecat:WIDTH", nsmgr),
                    Depth = XmlUtils.nodeAsDecimal(logisticDetailsNode, "./bmecat:PRODUCT_DIMENSIONS/bmecat:DEPTH", nsmgr),
                };
            }

            foreach (XmlNode referenceNode in XmlUtils.SelectNodes(productNode, "./bmecat:PRODUCT_REFERENCE", nsmgr))
            {
                product.References.Add(new Reference()
                {
                    Type = default(ReferenceTypes).FromString(XmlUtils.AttributeText(referenceNode, "type")),
                    IdTo = XmlUtils.nodeAsString(referenceNode, "./PROD_ID_TO", nsmgr)
                });
            }

            XmlNode extensionNode = XmlUtils.SelectSingleNode(productNode, "./bmecat:USER_DEFINED_EXTENSIONS", nsmgr);
            if (extensionNode != null)
            {
                if (extensionNode.InnerXml.Contains(".EDXF."))
                {
                    product.EDXF = _readEDXF(extensionNode, nsmgr);
                }

                product.ExtendedInformation = _readExtendedInformation(extensionNode, nsmgr);
            }

            return product;
        } // !_ReadProduct()


        private static List<Tuple<string,string>> _readExtendedInformation(XmlNode extensionNode, XmlNamespaceManager nsmgr)
        {
            List<Tuple<string, string>> retval = new List<Tuple<string, string>>();
            foreach (XmlNode node in extensionNode.ChildNodes)
            {
                if (node.Name.Contains(".EDXF"))
                {
                    continue;
                }

                string key = node.Name.Replace("UDX.", "");
                string value = node.InnerText;

                retval.Add(new Tuple<string, string>(key, value));
            }

            return retval;
        } // !_readExtendedInformation()


        private static EDXF _readEDXF(XmlNode extensionNode, XmlNamespaceManager nsmgr)
        {
            EDXF retval = new EDXF()
            {
                ManufacturerAcronym = XmlUtils.nodeAsString(extensionNode, "./bmecat:UDX.EDXF.MANUFACTURER_ACRONYM", nsmgr),
                ManufacturerDiscountGroups = XmlUtils.nodesAsStrings(extensionNode, "./bmecat:UDX.EDXF.DISCOUNT_GROUP/bmecat:UDX.EDXF.DISCOUNT_GROUP_MANUFACTURER", nsmgr),
                SupplierDiscountGroups = XmlUtils.nodesAsStrings(extensionNode, "./bmecat:UDX.EDXF.DISCOUNT_GROUP/bmecat:UDX.EDXF.DISCOUNT_GROUP_SUPPLIER", nsmgr),
                ProductSeries = XmlUtils.nodesAsStrings(extensionNode, "./bmecat:UDX.EDXF.PRODUCT_SERIES", nsmgr),
                ValidFrom = XmlUtils.nodeAsDateTime(extensionNode, "./bmecat:UDX.EDXF.VALID_FROM", nsmgr)
            };

            foreach(XmlNode packagingUnitNode in XmlUtils.SelectNodes(extensionNode, "./bmecat:UDX.EDXF.PACKING_UNITS/bmecat:UDX.EDXF.PACKING_UNIT", nsmgr))
            {
                PackagingUnit pu = new PackagingUnit()
                {
                    QuantityMin = XmlUtils.nodeAsInt(packagingUnitNode, "./bmecat:UDX.EDXF.QUANTITY_MIN", nsmgr),
                    QuantityMax = XmlUtils.nodeAsInt(packagingUnitNode, "./bmecat:UDX.EDXF.QUANTITY_MAX", nsmgr),
                    PackagingUnitCode = XmlUtils.nodeAsString(packagingUnitNode, "./bmecat:UDX.EDXF.PACKING_UNIT_CODE", nsmgr),
                    Weight = XmlUtils.nodeAsDecimal(packagingUnitNode, "./bmecat:UDX.EDXF.WEIGHT", nsmgr),
                    Length = XmlUtils.nodeAsDecimal(packagingUnitNode, "./bmecat:UDX.EDXF.LENGTH", nsmgr),
                    Width = XmlUtils.nodeAsDecimal(packagingUnitNode, "./bmecat:UDX.EDXF.WIDTH", nsmgr),
                    Depth = XmlUtils.nodeAsDecimal(packagingUnitNode, "./bmecat:UDX.EDXF.DEPTH", nsmgr),
                    GTIN = XmlUtils.nodeAsString(packagingUnitNode, "./bmecat:UDX.EDXF.GTIN", nsmgr)
                };

                retval.PackagingUnits.Add(pu);
            }

            XmlNode productLogisticsDetailsNode = XmlUtils.SelectSingleNode(extensionNode, "./bmecat:UDX.EDXF.PRODUCT_LOGISTIC_DETAILS", nsmgr);
            if (productLogisticsDetailsNode != null)
            {
                retval.ProductLogisticsDetails = new ProductLogisticsDetails()
                {
                    NetWeight = XmlUtils.nodeAsDecimal(productLogisticsDetailsNode, "./bmecat:UDX.EDXF.NETWEIGHT", nsmgr),
                    NetLength = XmlUtils.nodeAsDecimal(productLogisticsDetailsNode, "./bmecat:UDX.EDXF.NETLENGTH", nsmgr),
                    NetWidth = XmlUtils.nodeAsDecimal(productLogisticsDetailsNode, "./bmecat:UDX.EDXF.NETWIDTH", nsmgr),
                    NetDepth = XmlUtils.nodeAsDecimal(productLogisticsDetailsNode, "./bmecat:UDX.EDXF.NETDEPTH", nsmgr),
                    NetDiameter = XmlUtils.nodeAsDecimal(productLogisticsDetailsNode, "./bmecat:UDX.EDXF.NETDIAMETER", nsmgr),
                    RegionOfOrigin = XmlUtils.nodeAsString(productLogisticsDetailsNode, "./bmecat:REGION_OF_ORIGIN", nsmgr)
                };                
            }

            foreach(XmlNode mimeNode in XmlUtils.SelectNodes(extensionNode, "./bmecat:UDX.EDXF.MIME_INFO/bmecat:UDX.EDXF.MIME", nsmgr))
            {
                retval.MimeInfos.Add(new MimeInfo()
                {
                    Source = XmlUtils.nodeAsString(mimeNode, "./bmecat:UDX.EDXF.MIME_SOURCE", nsmgr),
                    Alt = XmlUtils.nodeAsString(mimeNode, "./bmecat:UDX.EDXF.MIME_ALT", nsmgr),
                    Description = XmlUtils.nodeAsString(mimeNode, "./bmecat:UDX.EDXF.MIME_DESIGNATION", nsmgr)
                });
            }

            retval.Reach = new Reach()
            {
                Info = XmlUtils.nodeAsString(extensionNode, "./bmecat:UDX.EDXF.REACH/bmecat:UDX.EDXF.REACH.INFO", nsmgr),
                ListDate = XmlUtils.nodeAsDateTime(extensionNode, "./bmecat:UDX.EDXF.REACH/bmecat:UDX.EDXF.REACH.LISTDATE", nsmgr)
            };

            return retval;
        } // !_readEDXF()


        private static Party _ReadPartyAddress(XmlNode addressNode, XmlNamespaceManager nsmgr)
        {
            Party p = new Party()
            {
                Name2 = XmlUtils.nodeAsString(addressNode, "./bmecat:NAME2"),
                Name3 = XmlUtils.nodeAsString(addressNode, "./bmecat:NAME2"),
                Department = XmlUtils.nodeAsString(addressNode, "./bmecat:DEPARTMENT"),
                ContactName = XmlUtils.nodeAsString(addressNode, "./bmecat:CONTACT"),
                Street = XmlUtils.nodeAsString(addressNode, "./bmecat:STREET"),
                Zip = XmlUtils.nodeAsString(addressNode, "./bmecat:ZIP"),
                BoxNo = XmlUtils.nodeAsString(addressNode, "./bmecat:BOXNO"),
                ZipBox = XmlUtils.nodeAsString(addressNode, "./bmecat:ZIPBOX"),
                City = XmlUtils.nodeAsString(addressNode, "./bmecat:CITY"),
                State = XmlUtils.nodeAsString(addressNode, "./bmecat:STATE"),
                Country = XmlUtils.nodeAsString(addressNode, "./bmecat:COUNTRY"),
                VATID = XmlUtils.nodeAsString(addressNode, "./bmecat:VAT_ID"),
                Phone = XmlUtils.nodeAsString(addressNode, "./bmecat:PHONE"), // @todo phone is typed
                Fax = XmlUtils.nodeAsString(addressNode, "./bmecat:FAX") // @todo fax is typed
            };

            foreach (XmlNode contactDetailNode in XmlUtils.SelectNodes(addressNode, "./bmecat:CONTACT_DETAILS"))
            {
                p.ContactDetails.Add(new ContactDetails()
                {
                    Id = XmlUtils.nodeAsString(contactDetailNode, "./bmecat:CONTACT_ID"),
                    Name = XmlUtils.nodeAsString(contactDetailNode, "./bmecat:CONTACT_NAME"),
                    FirstName = XmlUtils.nodeAsString(contactDetailNode, "./bmecat:FIRST_NAME"),
                    Title = XmlUtils.nodeAsString(contactDetailNode, "./bmecat:TITLE"),
                    AcademicTitle = XmlUtils.nodeAsString(contactDetailNode, "./bmecat:ACADEMIC_TITLE"),
                    ContactRole = XmlUtils.nodeAsString(contactDetailNode, "./bmecat:CONTACT_ROLE"), // @todo role is typed
                    ContactDescription = XmlUtils.nodeAsString(contactDetailNode, "./bmecat:CONTACT_DESCR"),
                    Phone = XmlUtils.nodesAsStrings(contactDetailNode, "./bmecat:PHONE"), // @todo phone is typed
                    Fax = XmlUtils.nodesAsStrings(contactDetailNode, "./bmecat:FAX"), // @todo fax is typed
                    URL = XmlUtils.nodeAsString(contactDetailNode, "./bmecat:URL"),
                    Emails = XmlUtils.nodeAsString(contactDetailNode, "./bmecat:EMAILS") // @todo find out if this is a list in reality 
                });
            }

            return p;
        } // !_ReadyPartyAddress()
    }
}