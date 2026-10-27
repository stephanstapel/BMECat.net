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
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.XPath;

namespace BMECat.net
{
    internal class BMECatReader12 : BMECatReaderBase
    {
        internal static Task<ProductCatalog> LoadAsync(Stream inputStream, BMECatExtensions extensions = null)
        {
            // reading works on the in memory document and has nothing to await
            return Task.FromResult(_Load(inputStream, extensions));
        } // !LoadAsync()


        private static ProductCatalog _Load(Stream inputStream, BMECatExtensions extensions)
        {
            if (inputStream == null)
            {
                return null;
            }

            XmlDocument doc = _LoadDocument(inputStream, @"<BMECAT xmlns=""http://www.bmecat.org/bmecat:bmecat/bmecat:1.2/bmecat:bmecat_new_catalog"" version=""1.2"">", extensions);
            XmlNamespaceManager nsmgr = _CreateNamespaceManager(doc);

            ProductCatalog retval = new ProductCatalog();

            foreach(XmlNode languageNode in doc.SelectNodes("/bmecat:BMECAT/bmecat:HEADER/bmecat:CATALOG/bmecat:LANGUAGE", nsmgr))
            {
                LanguageCodes? language = default(LanguageCodes).FromString(languageNode.InnerText);
                if (language.HasValue)
                {
                    retval.Languages.Add(language.Value);
                }
            }

            retval.CatalogId = XmlUtils.nodeAsString(doc, "/bmecat:BMECAT/bmecat:HEADER/bmecat:CATALOG/bmecat:CATALOG_ID", nsmgr);
            retval.CatalogVersion = XmlUtils.nodeAsString(doc, "/bmecat:BMECAT/bmecat:HEADER/bmecat:CATALOG/bmecat:CATALOG_VERSION", nsmgr);
            retval.CatalogName = XmlUtils.nodeAsString(doc.DocumentElement, "/bmecat:BMECAT/bmecat:HEADER/bmecat:CATALOG/bmecat:CATALOG_NAME", nsmgr);
            retval.GenerationDate = XmlUtils.nodeAsDateTime(doc.DocumentElement, "/bmecat:BMECAT/bmecat:HEADER/bmecat:CATALOG/bmecat:GENERATION_DATE", nsmgr);
            retval.Currency = default(CurrencyCodes).FromString(XmlUtils.nodeAsString(doc.DocumentElement, "/bmecat:BMECAT/bmecat:HEADER/bmecat:CATALOG/bmecat:CURRENCY", nsmgr));

            XmlNode agreementNode = doc.SelectSingleNode("/bmecat:BMECAT/bmecat:HEADER/bmecat:AGREEMENT", nsmgr);
            if (agreementNode != null)
            {
                retval.Agreement = new Agreement()
                {
                    Id = XmlUtils.nodeAsString(agreementNode, "./bmecat:AGREEMENT_ID", nsmgr),
                    StartDate = XmlUtils.nodeAsDateTime(agreementNode, "./bmecat:AGREEMENT_START_DATE", nsmgr),
                    EndDate = XmlUtils.nodeAsDateTime(agreementNode, "./bmecat:AGREEMENT_END_DATE", nsmgr),
                };
            }

            XmlNode buyerNode = doc.SelectSingleNode("/bmecat:BMECAT/bmecat:HEADER/bmecat:BUYER", nsmgr);
            if (buyerNode != null)
            {
                XmlNode addressNode = buyerNode.SelectSingleNode("./bmecat:ADDRESS", nsmgr);
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

            XmlNode supplierNode = doc.SelectSingleNode("/bmecat:BMECAT/bmecat:HEADER/bmecat:SUPPLIER", nsmgr);
            if (supplierNode != null)
            {

                XmlNode addressNode = supplierNode.SelectSingleNode("./bmecat:ADDRESS", nsmgr);
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

            // XmlDocument is not thread safe, not even for reading, hence the articles are read one after another
            foreach (XmlNode productNode in doc.DocumentElement.SelectNodes("/bmecat:BMECAT/bmecat:T_NEW_CATALOG/bmecat:ARTICLE", nsmgr))
            {
                retval.Products.Add(_ReadProduct(productNode, nsmgr, extensions));
            }

            foreach (XmlNode catalogNode in doc.DocumentElement.SelectNodes("/bmecat:BMECAT/bmecat:T_NEW_CATALOG/bmecat:CATALOG_GROUP_SYSTEM/bmecat:CATALOG_STRUCTURE", nsmgr))
            {
                retval.CatalogStructures.Add(_ReadCatalogStructure(catalogNode, nsmgr));
            }

            // -- map catalog group assignments to products
            Dictionary<string, List<ProductCatalogGroupMapping>> mappingsMap = new Dictionary<string, List<ProductCatalogGroupMapping>>();

            XmlNodeList articleToCatalogGroupMapNodes = doc.DocumentElement.SelectNodes("/bmecat:BMECAT/bmecat:T_NEW_CATALOG/bmecat:ARTICLE_TO_CATALOGGROUP_MAP", nsmgr);
            foreach (XmlNode articleToCatalogGroupMapNode in articleToCatalogGroupMapNodes)
            {
                string articleId = XmlUtils.nodeAsString(articleToCatalogGroupMapNode, "./bmecat:ART_ID", nsmgr);

                if (!mappingsMap.ContainsKey(articleId))
                {
                    mappingsMap.Add(articleId, new List<ProductCatalogGroupMapping>());
                }

                mappingsMap[articleId].Add(new ProductCatalogGroupMapping()
                {
                    /**
                     * @todo read optional SUPPLIER_IDREF sub structure
                     */

                    CatalogGroupId = XmlUtils.nodeAsString(articleToCatalogGroupMapNode, "./bmecat:CATALOG_GROUP_ID", nsmgr),
                    Order = XmlUtils.nodeAsInt(articleToCatalogGroupMapNode, "./bmecat:ARTICLE_TO_CATALOGGROUP_MAP_ORDER", nsmgr)
                });
            }

            foreach(Product p in retval.Products)
            {
                if (mappingsMap.ContainsKey(p.No))
                {
                    p.ProductCatalogGroupMappings = mappingsMap[p.No];
                }
            }

            return retval;
        } // !_Load()


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

            foreach(XmlNode mimeNode in catalogNode.SelectNodes("./bmecat:MIME_INFO/bmecat:MIME", nsmgr))
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


        private static Product _ReadProduct(XmlNode productNode, XmlNamespaceManager nsmgr, BMECatExtensions extensions = null)
        {
            string productModeAsString = XmlUtils.nodeAsString(productNode, "@mode", nsmgr);

            Product product = new Product()
            {
                No = XmlUtils.nodeAsString(productNode, "./bmecat:SUPPLIER_AID", nsmgr),
                DescriptionShort = XmlUtils.nodeAsString(productNode, "./bmecat:ARTICLE_DETAILS/bmecat:DESCRIPTION_SHORT", nsmgr),
                DescriptionLong = XmlUtils.nodeAsString(productNode, "./bmecat:ARTICLE_DETAILS/bmecat:DESCRIPTION_LONG", nsmgr),                
                Stock = XmlUtils.nodeAsInt(productNode, "./bmecat:ARTICLE_DETAILS/bmecat:STOCK", nsmgr),                                
                SupplierAltPid = XmlUtils.nodeAsString(productNode, "./bmecat:ARTICLE_DETAILS/bmecat:SUPPLIER_ALT_AID", nsmgr),
                ManufacturerPID = XmlUtils.nodeAsString(productNode, "./bmecat:ARTICLE_DETAILS/bmecat:MANUFACTURER_AID", nsmgr),
                ManufacturerName = XmlUtils.nodeAsString(productNode, "./bmecat:ARTICLE_DETAILS/bmecat:MANUFACTURER_NAME", nsmgr),
                ManufacturerTypeDescription = XmlUtils.nodeAsString(productNode, "./bmecat:ARTICLE_DETAILS/bmecat:MANUFACTURER_TYPE_DESCR", nsmgr),
                ERPGroupSupplier = XmlUtils.nodeAsString(productNode, "./bmecat:ARTICLE_DETAILS/bmecat:ERP_GROUP_SUPPLIER", nsmgr),
                ERPGroupBuyer = XmlUtils.nodeAsString(productNode, "./bmecat:ARTICLE_DETAILS/bmecat:ERP_GROUP_BUYER", nsmgr),
            };

            string eanCode = XmlUtils.nodeAsString(productNode, "./bmecat:ARTICLE_DETAILS/bmecat:EAN", nsmgr);
            if (!String.IsNullOrEmpty(eanCode))
            {
                product.PIds.Add(new ProductId()
                {
                    Type = ProductIdTypes.EAN,
                    Id = eanCode,
                });
            }
            

            /*
             * @TODO 
             * <SPECIAL_TREATMENT_CLASS type="SIDAB">true</bmecat:SPECIAL_TREATMENT_CLASS>
                <SPECIAL_TREATMENT_CLASS type="UN">3481</bmecat:SPECIAL_TREATMENT_CLASS>
             */

            foreach (XmlNode supplierPIdNode in productNode.SelectNodes("./bmecat:SUPPLIER_AID", nsmgr))
            {
                product.SupplierPIds.Add(new ProductId()
                {
                    Type = default(ProductIdTypes).FromString(XmlUtils.AttributeText(supplierPIdNode, "type")),
                    Id = supplierPIdNode.InnerText
                });
            }

            foreach (XmlNode keywordNode in productNode.SelectNodes("./bmecat:ARTICLE_DETAILS/bmecat:KEYWORD", nsmgr))
            {
                product.Keywords.Add(keywordNode.InnerText);
            }

            // parse features including classification
            foreach(XmlNode articleFeaturesNode in productNode.SelectNodes("./bmecat:ARTICLE_FEATURES", nsmgr))
            {
                FeatureSet featureSet = new FeatureSet();

                XmlNode classificationNode = articleFeaturesNode.SelectSingleNode("./bmecat:REFERENCE_FEATURE_SYSTEM_NAME", nsmgr);
                if (classificationNode != null)
                {
                    if (featureSet.FeatureClassificationSystem == null) { featureSet.FeatureClassificationSystem = new FeatureClassificationSystem(); }
                    featureSet.FeatureClassificationSystem.Classification = classificationNode.InnerText;
                }

                XmlNode classifictionGroupName = articleFeaturesNode.SelectSingleNode("./bmecat:REFERENCE_FEATURE_GROUP_NAME", nsmgr);
                if (classifictionGroupName != null)
                {
                    if (featureSet.FeatureClassificationSystem == null) { featureSet.FeatureClassificationSystem = new FeatureClassificationSystem(); }
                    featureSet.FeatureClassificationSystem.GroupName = classifictionGroupName.InnerText;
                }

                foreach (XmlNode classifictionGroupId in articleFeaturesNode.SelectNodes("./bmecat:REFERENCE_FEATURE_GROUP_ID", nsmgr))
                {
                    if (featureSet.FeatureClassificationSystem == null) { featureSet.FeatureClassificationSystem = new FeatureClassificationSystem(); }
                    featureSet.FeatureClassificationSystem.GroupIds.Add(new FeatureClassificationSystemGroupId()
                    {
                        Name = classifictionGroupId.InnerText
                    });
                }

                // parse features
                foreach (XmlNode featureNode in articleFeaturesNode.SelectNodes("./bmecat:FEATURE", nsmgr))
                {
                    featureSet.Features.Add(new Feature()
                    {
                        Name = XmlUtils.nodeAsString(featureNode, "./bmecat:FNAME", nsmgr),
                        Values = XmlUtils.nodesAsStrings(featureNode, "./bmecat:FVALUE", nsmgr),
                        Unit = _convertQuantityCode(XmlUtils.nodeAsString(featureNode, "./bmecat:FUNIT", nsmgr, null), extensions),
                        Order = XmlUtils.nodeAsString(featureNode, "./bmecat:FORDER", nsmgr)
                        /* TODO: FVALUE_TYPE */
                    });
                }

                product.FeatureSets.Add(featureSet);
            }            

            XmlNode orderDetailsNode = productNode.SelectSingleNode("./bmecat:ARTICLE_ORDER_DETAILS", nsmgr);
            if (orderDetailsNode != null)
            {
                product.OrderDetails = new OrderDetails()
                {
                    OrderUnit = _convertQuantityCode(XmlUtils.nodeAsString(productNode, "./bmecat:ARTICLE_ORDER_DETAILS/bmecat:ORDER_UNIT", nsmgr, null), extensions),
                    ContentUnit = _convertQuantityCode(XmlUtils.nodeAsString(productNode, "./bmecat:ARTICLE_ORDER_DETAILS/bmecat:CONTENT_UNIT", nsmgr, null), extensions),
                    ContentUnitPerOrderUnit = XmlUtils.nodeAsInt(orderDetailsNode, "./bmecat:NO_CU_PER_OU", nsmgr),
                    PriceQuantity = XmlUtils.nodeAsInt(orderDetailsNode, "./bmecat:PRICE_QUANTITY", nsmgr),
                    QuantityMin = XmlUtils.nodeAsInt(orderDetailsNode, "./bmecat:QUANTITY_MIN", nsmgr),
                    QuantityInterval = XmlUtils.nodeAsInt(orderDetailsNode, "./bmecat:QUANTITY_INTERVAL", nsmgr),
                };
            }

            foreach (XmlNode priceDetailNode in productNode.SelectNodes("./bmecat:ARTICLE_PRICE_DETAILS/bmecat:ARTICLE_PRICE", nsmgr))
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

            foreach (XmlNode mimeNode in productNode.SelectNodes("./bmecat:MIME_INFO/bmecat:MIME", nsmgr))
            {
                product.MimeInfos.Add(new MimeInfo()
                {
                    MimeType = default(MimeTypes).FromString(XmlUtils.nodeAsString(mimeNode, "./bmecat:MIME_TYPE", nsmgr)),
                    Source = XmlUtils.nodeAsString(mimeNode, "./bmecat:MIME_SOURCE", nsmgr),
                    Description = XmlUtils.nodeAsString(mimeNode, "./bmecat:MIME_DESCR", nsmgr),
                    Purpose = XmlUtils.nodeAsString(mimeNode, "./bmecat:MIME_PURPOSE", nsmgr),
                    Order = XmlUtils.nodeAsInt(mimeNode, "./bmecat:MIME_ORDER", nsmgr)
                });
            }

            XmlNode logisticDetailsNode = productNode.SelectSingleNode("./bmecat:PRODUCT_LOGISTIC_DETAILS", nsmgr);
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
            else
            {
                logisticDetailsNode = productNode.SelectSingleNode("./bmecat:ARTICLE_LOGISTIC_DETAILS", nsmgr);
                if (logisticDetailsNode != null)
                {
                    product.LogisticsDetails = new LogisticsDetails()
                    {
                        CountryOfOrigin = default(CountryCodes).FromString(XmlUtils.nodeAsString(logisticDetailsNode, "./bmecat:COUNTRY_OF_ORIGIN", nsmgr)),
                        CustomsTariffNumber = XmlUtils.nodesAsStrings(logisticDetailsNode, "./bmecat:CUSTOMS_TARIFF_NUMBER", nsmgr),
                        Volume = XmlUtils.nodeAsDecimal(logisticDetailsNode, "./bmecat:ARTICLE_DIMENSIONS/bmecat:VOLUME", nsmgr),
                        Weight = XmlUtils.nodeAsDecimal(logisticDetailsNode, "./bmecat:ARTICLE_DIMENSIONS/bmecat:WEIGHT", nsmgr),
                        Length = XmlUtils.nodeAsDecimal(logisticDetailsNode, "./bmecat:ARTICLE_DIMENSIONS/bmecat:LENGTH", nsmgr),
                        Width = XmlUtils.nodeAsDecimal(logisticDetailsNode, "./bmecat:ARTICLE_DIMENSIONS/bmecat:WIDTH", nsmgr),
                        Depth = XmlUtils.nodeAsDecimal(logisticDetailsNode, "./bmecat:ARTICLE_DIMENSIONS/bmecat:DEPTH", nsmgr),
                    };
                }
            }

            foreach(XmlNode referenceNode in productNode.SelectNodes("./bmecat:ARTICLE_REFERENCE", nsmgr))
            {
                product.References.Add(new Reference()
                {
                    Type = default(ReferenceTypes).FromString(XmlUtils.AttributeText(referenceNode, "type")),
                    IdTo = XmlUtils.nodeAsString(referenceNode, "./bmecat:ART_ID_TO", nsmgr)
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



        private static List<Tuple<string, string>> _readExtendedInformation(XmlNode extensionNode, XmlNamespaceManager nsmgr)
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

            foreach (XmlNode packagingUnitNode in XmlUtils.SelectNodes(extensionNode, "./bmecat:UDX.EDXF.PACKING_UNITS/bmecat:UDX.EDXF.PACKING_UNIT", nsmgr))
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
                    RegionOfOrigin = XmlUtils.nodeAsString(productLogisticsDetailsNode, "./bmecat:UDX.EDXF.REGION_OF_ORIGIN", nsmgr)
                };
            }

            foreach (XmlNode mimeNode in XmlUtils.SelectNodes(extensionNode, "./bmecat:UDX.EDXF.MIME_INFO/bmecat:UDX.EDXF.MIME", nsmgr))
            {
                retval.MimeInfos.Add(new MimeInfo()
                {
                    Source = XmlUtils.nodeAsString(mimeNode, "./bmecat:UDX.EDXF.MIME_SOURCE", nsmgr),
                    Alt = XmlUtils.nodeAsString(mimeNode, "./bmecat:UDX.EDXF.MIME_ALT", nsmgr),
                    Description = XmlUtils.nodeAsString(mimeNode, "./bmecat:UDX.EDXF.MIME_DESIGNATION", nsmgr),
                    Filename = XmlUtils.nodeAsString(mimeNode, "./bmecat:UDX.EDXF.MIME_FILENAME", nsmgr),
                    Code = XmlUtils.nodeAsString(mimeNode, "./bmecat:UDX.EDXF.MIME_CODE", nsmgr),
                });
            }

            retval.Reach = new Reach()
            {
                Info = XmlUtils.nodeAsString(extensionNode, "./bmecat:UDX.EDXF.REACH/bmecat:UDX.EDXF.REACH.INFO", nsmgr),
                ListDate = XmlUtils.nodeAsDateTime(extensionNode, "./bmecat:UDX.EDXF.REACH/bmecat:UDX.EDXF.REACH.LISTDATE", nsmgr)
            };

            return retval;
        } // !_readEDXF()
    }
}
