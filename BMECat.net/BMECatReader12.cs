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
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.XPath;

namespace BMECat.net
{
    internal class BMECatReader12 : BMECatReaderBase
    {
        internal static ProductCatalog Load(XmlDocument doc, BMECatExtensions extensions = null)
        {   
            XmlNamespaceManager nsmgr = new XmlNamespaceManager(doc.DocumentElement.OwnerDocument.NameTable);
            nsmgr.AddNamespace("xsi", "http://www.bmecat.org/bmecat/1.2/bmecat_new_catalog");

            ProductCatalog retval = new ProductCatalog();

            foreach(XmlNode languageNode in doc.SelectNodes("/BMECAT/HEADER/CATALOG/LANGUAGE", nsmgr))
            {
                LanguageCodes language = default(LanguageCodes).FromString(languageNode.InnerText);
                retval.Languages.Add(language);
            }

            retval.CatalogId = XmlUtils.nodeAsString(doc, "/BMECAT/HEADER/CATALOG/CATALOG_ID", nsmgr);
            retval.CatalogVersion = XmlUtils.nodeAsString(doc, "/BMECAT/HEADER/CATALOG/CATALOG_VERSION", nsmgr);
            retval.CatalogName = XmlUtils.nodeAsString(doc.DocumentElement, "/BMECAT/HEADER/CATALOG/CATALOG_NAME", nsmgr);
            retval.GenerationDate = XmlUtils.nodeAsDateTime(doc.DocumentElement, "/BMECAT/HEADER/CATALOG/GENERATION_DATE", nsmgr);
            retval.Currency = default(CurrencyCodes).FromString(XmlUtils.nodeAsString(doc.DocumentElement, "/BMECAT/HEADER/CATALOG/CURRENCY", nsmgr));

            XmlNode agreementNode = doc.SelectSingleNode("/BMECAT/HEADER/AGREEMENT", nsmgr);
            if (agreementNode != null)
            {
                retval.Agreement = new Agreement()
                {
                    Id = XmlUtils.nodeAsString(agreementNode, "./AGREEMENT_ID", nsmgr),
                    StartDate = XmlUtils.nodeAsDateTime(agreementNode, "./AGREEMENT_START_DATE", nsmgr),
                    EndDate = XmlUtils.nodeAsDateTime(agreementNode, "./AGREEMENT_END_DATE", nsmgr),
                };
            }

            XmlNode buyerNode = doc.SelectSingleNode("/BMECAT/HEADER/BUYER", nsmgr);
            if (buyerNode != null)
            {
                XmlNode addressNode = doc.SelectSingleNode("./ADDRESS", nsmgr);
                if (addressNode != null)
                {
                    retval.Buyer = _ReadPartyAddress(addressNode, nsmgr);                    
                }
                else
                {
                    retval.Buyer = new Party();
                }

                /* @todo regard id types */
                retval.Buyer.Id = XmlUtils.nodeAsString(buyerNode, "./BUYER_ID", nsmgr);
                retval.Buyer.Name = XmlUtils.nodeAsString(buyerNode, "./BUYER_NAME", nsmgr);                
            }

            XmlNode supplierNode = doc.SelectSingleNode("/BMECAT/HEADER/SUPPLIER", nsmgr);
            if (supplierNode != null)
            {

                XmlNode addressNode = doc.SelectSingleNode("./ADDRESS", nsmgr);
                if (addressNode != null)
                {
                    retval.Supplier = _ReadPartyAddress(addressNode, nsmgr);
                }
                else
                {
                    retval.Supplier = new Party();
                }

                /* @todo regard id types */
                retval.Supplier.Id = XmlUtils.nodeAsString(supplierNode, "./SUPPLIER_ID", nsmgr);
                retval.Supplier.Name = XmlUtils.nodeAsString(supplierNode, "./SUPPLIER_NAME", nsmgr);
            }

            XmlNodeList productNodes = doc.DocumentElement.SelectNodes("/BMECAT/T_NEW_CATALOG/ARTICLE", nsmgr);
            Parallel.ForEach(productNodes.Cast<XmlNode>(), /* new ParallelOptions() {  MaxDegreeOfParallelism = 1 }, */
                             (XmlNode productNode) =>
            {
                Product product = _ReadProduct(productNode, nsmgr, extensions);
                retval.Products.Add(product);
            });

            XmlNodeList catalogNodes = doc.DocumentElement.SelectNodes("/BMECAT/T_NEW_CATALOG/CATALOG_GROUP_SYSTEM/CATALOG_STRUCTURE", nsmgr);
            Parallel.ForEach(catalogNodes.Cast<XmlNode>(), /* new ParallelOptions() {  MaxDegreeOfParallelism = 1 }, */
                             (XmlNode catalogNode) =>
            {
                CatalogStructure catalogStructure = _ReadCatalogStructure(catalogNode, nsmgr);
                retval.CatalogStructures.Add(catalogStructure);
            });

            return retval;
        } // !Load()


        private static CatalogStructure _ReadCatalogStructure(XmlNode catalogNode, XmlNamespaceManager nsmgr)
        {
            CatalogStructure catalogStructure = new CatalogStructure()
            {
                Type = default(CatalogStructureTypes).FromString(XmlUtils.AttributeText(catalogNode, "type")),
                GroupId = XmlUtils.nodeAsString(catalogNode, "./GROUP_ID", nsmgr),
                GroupName = XmlUtils.nodeAsString(catalogNode, "./GROUP_NAME", nsmgr),
                ParentId = XmlUtils.nodeAsString(catalogNode, "./PARENT_ID", nsmgr),
                GroupOrder = XmlUtils.nodeAsString(catalogNode, "./GROUP_ORDER", nsmgr),
            };

            foreach(XmlNode mimeNode in catalogNode.SelectNodes("./MIME_INFO/MIME", nsmgr))
            {
                catalogStructure.MimeInfos.Add(new MimeInfo()
                {
                    MimeType = default(MimeTypes).FromString(XmlUtils.nodeAsString(mimeNode, "./MIME_TYPE", nsmgr)),
                    Source = XmlUtils.nodeAsString(mimeNode, "./MIME_SOURCE", nsmgr),
                    Description = XmlUtils.nodeAsString(mimeNode, "./MIME_DESCR", nsmgr),
                    Purpose = XmlUtils.nodeAsString(mimeNode, "./MIME_PURPOSE", nsmgr),
                    Alt = XmlUtils.nodeAsString(mimeNode, "./MIME_ALT", nsmgr),
                    Order = XmlUtils.nodeAsInt(mimeNode, "./MIME_ORDER", nsmgr),
                });
            }

            return catalogStructure;
        } // !_ReadCatalogStructure()


        private static Product _ReadProduct(XmlNode productNode, XmlNamespaceManager nsmgr, BMECatExtensions extensions = null)
        {
            string _productMode = XmlUtils.nodeAsString(productNode, "@mode", nsmgr);

            Product product = new Product()
            {
                No = XmlUtils.nodeAsString(productNode, "./SUPPLIER_PID", nsmgr),
                DescriptionShort = XmlUtils.nodeAsString(productNode, "./ARTICLE_DETAILS/DESCRIPTION_SHORT", nsmgr),
                DescriptionLong = XmlUtils.nodeAsString(productNode, "./ARTICLE_DETAILS/DESCRIPTION_LONG", nsmgr),
                EANCode = XmlUtils.nodeAsString(productNode, "./ARTICLE_DETAILS/EAN", nsmgr),
                Stock = XmlUtils.nodeAsInt(productNode, "./ARTICLE_DETAILS/STOCK", nsmgr),                                
                SupplierAltPid = XmlUtils.nodeAsString(productNode, "./ARTICLE_DETAILS/SUPPLIER_ALT_AID", nsmgr),
                ManufacturerPID = XmlUtils.nodeAsString(productNode, "./ARTICLE_DETAILS/MANUFACTURER_AID", nsmgr),
                ManufacturerName = XmlUtils.nodeAsString(productNode, "./ARTICLE_DETAILS/MANUFACTURER_NAME", nsmgr),
                ManufacturerTypeDescription = XmlUtils.nodeAsString(productNode, "./ARTICLE_DETAILS/MANUFACTURER_TYPE_DESCR", nsmgr),
                ERPGroupSupplier = XmlUtils.nodeAsString(productNode, "./ARTICLE_DETAILS/ERP_GROUP_SUPPLIER", nsmgr),
                ERPGroupBuyer = XmlUtils.nodeAsString(productNode, "./ARTICLE_DETAILS/ERP_GROUP_BUYER", nsmgr),                
            };

            /*
             * @TODO 
             * <SPECIAL_TREATMENT_CLASS type="SIDAB">true</SPECIAL_TREATMENT_CLASS>
                <SPECIAL_TREATMENT_CLASS type="UN">3481</SPECIAL_TREATMENT_CLASS>
             */

            foreach (XmlNode supplierPIdNode in productNode.SelectNodes("./SUPPLIER_PID", nsmgr))
            {
                product.SupplierPIds.Add(new SupplierProductId()
                {
                    Type = default(SupplierProductIdTypes).FromString(XmlUtils.AttributeText(supplierPIdNode, "type")),
                    Id = supplierPIdNode.InnerText
                });
            }

            foreach (XmlNode keywordNode in productNode.SelectNodes("./ARTICLE_DETAILS/KEYWORD", nsmgr))
            {
                product.Keywords.Add(keywordNode.InnerText);
            }

            foreach (XmlNode featureNode in productNode.SelectNodes("./ARTICLE_FEATURES/FEATURE", nsmgr))
            {
                product.ProductFeatures.Add(new Feature()
                {
                    Name = XmlUtils.nodeAsString(featureNode, "./FNAME", nsmgr),
                    Values = XmlUtils.nodesAsStrings(featureNode, "./FVALUE", nsmgr),
                    Unit = _convertQuantityCode(XmlUtils.nodeAsString(featureNode, "./FUNIT", nsmgr, null), extensions),
                    Order = XmlUtils.nodeAsString(featureNode, "./FORDER", nsmgr)
                    /* TODO: FVALUE_TYPE */
                });
            }

            XmlNode orderDetailsNode = productNode.SelectSingleNode("./ARTICLE_ORDER_DETAILS", nsmgr);
            if (orderDetailsNode != null)
            {
                product.OrderDetails = new OrderDetails()
                {
                    OrderUnit = _convertQuantityCode(XmlUtils.nodeAsString(productNode, "./ARTICLE_ORDER_DETAILS/ORDER_UNIT", nsmgr, null), extensions),
                    ContentUnit = _convertQuantityCode(XmlUtils.nodeAsString(productNode, "./ARTICLE_ORDER_DETAILS/CONTENT_UNIT", nsmgr, null), extensions),
                    ContentUnitPerOrderUnit = XmlUtils.nodeAsInt(orderDetailsNode, "./NO_CU_PER_OU", nsmgr),
                    PriceQuantity = XmlUtils.nodeAsInt(orderDetailsNode, "./PRICE_QUANTITY", nsmgr),
                    QuantityMin = XmlUtils.nodeAsInt(orderDetailsNode, "./QUANTITY_MIN", nsmgr),
                    QuantityInterval = XmlUtils.nodeAsInt(orderDetailsNode, "./QUANTITY_INTERVAL", nsmgr),
                };
            }

            foreach (XmlNode priceDetailNode in productNode.SelectNodes("./ARTICLE_PRICE_DETAILS/ARTICLE_PRICE", nsmgr))
            {
                product.Prices.Add(new ProductPrice()
                {
                    PriceType = default(PriceTypes).FromString(XmlUtils.AttributeText(priceDetailNode, "price_type")),
                    Amount = XmlUtils.nodeAsDecimal(priceDetailNode, "./PRICE_AMOUNT", nsmgr, 0).Value,
                    Currency = default(CurrencyCodes).FromString(XmlUtils.nodeAsString(priceDetailNode, "./PRICE_CURRENCY", nsmgr)),
                    Tax = XmlUtils.nodeAsDecimal(priceDetailNode, "./TAX", nsmgr, 0).Value,
                    LowerBound = XmlUtils.nodeAsInt(priceDetailNode, "./LOWER_BOUND", nsmgr)
                });
            }

            foreach (XmlNode mimeNode in productNode.SelectNodes("./MIME_INFO/MIME", nsmgr))
            {
                product.MimeInfos.Add(new MimeInfo()
                {
                    MimeType = default(MimeTypes).FromString(XmlUtils.nodeAsString(mimeNode, "./MIME_TYPE", nsmgr)),
                    Source = XmlUtils.nodeAsString(mimeNode, "./MIME_SOURCE", nsmgr),
                    Description = XmlUtils.nodeAsString(mimeNode, "./MIME_DESCR", nsmgr),
                    Purpose = XmlUtils.nodeAsString(mimeNode, "./MIME_PURPOSE", nsmgr),
                    Order = XmlUtils.nodeAsInt(mimeNode, "./MIME_ORDER", nsmgr)
                });
            }

            XmlNode logisticDetailsNode = productNode.SelectSingleNode("./PRODUCT_LOGISTIC_DETAILS", nsmgr);
            if (logisticDetailsNode != null)
            {
                product.LogisticsDetails = new LogisticsDetails()
                {
                    CountryOfOrigin = default(CountryCodes).FromString(XmlUtils.nodeAsString(logisticDetailsNode, "./COUNTRY_OF_ORIGIN", nsmgr)),
                    CustomsTariffNumber = XmlUtils.nodesAsStrings(logisticDetailsNode, "./CUSTOMS_TARIFF_NUMBER", nsmgr),
                    Volume = XmlUtils.nodeAsDecimal(logisticDetailsNode, "./PRODUCT_DIMENSIONS/VOLUME", nsmgr),
                    Weight = XmlUtils.nodeAsDecimal(logisticDetailsNode, "./PRODUCT_DIMENSIONS/WEIGHT", nsmgr),
                    Length = XmlUtils.nodeAsDecimal(logisticDetailsNode, "./PRODUCT_DIMENSIONS/LENGTH", nsmgr),
                    Width = XmlUtils.nodeAsDecimal(logisticDetailsNode, "./PRODUCT_DIMENSIONS/WIDTH", nsmgr),
                    Depth = XmlUtils.nodeAsDecimal(logisticDetailsNode, "./PRODUCT_DIMENSIONS/DEPTH", nsmgr),
                };
            }

            foreach(XmlNode referenceNode in productNode.SelectNodes("./ARTICLE_REFERENCE", nsmgr))
            {
                product.References.Add(new Reference()
                {
                    Type = default(ReferenceTypes).FromString(XmlUtils.AttributeText(referenceNode, "type")),
                    IdTo = XmlUtils.nodeAsString(referenceNode, "./ART_ID_TO", nsmgr)
                });
            }

            return product;
        } // !_ReadProduct()


        private static Party _ReadPartyAddress(XmlNode addressNode, XmlNamespaceManager nsmgr)
        {
            Party p = new Party()
            {
                Name2 = XmlUtils.nodeAsString(addressNode, "./NAME2"),
                Name3 = XmlUtils.nodeAsString(addressNode, "./NAME2"),
                Department = XmlUtils.nodeAsString(addressNode, "./DEPARTMENT"),
                ContactName = XmlUtils.nodeAsString(addressNode, "./CONTACT"),
                Street = XmlUtils.nodeAsString(addressNode, "./STREET"),
                Zip = XmlUtils.nodeAsString(addressNode, "./ZIP"),
                BoxNo = XmlUtils.nodeAsString(addressNode, "./BOXNO"),
                ZipBox = XmlUtils.nodeAsString(addressNode, "./ZIPBOX"),
                City = XmlUtils.nodeAsString(addressNode, "./CITY"),
                State = XmlUtils.nodeAsString(addressNode, "./STATE"),
                Country = XmlUtils.nodeAsString(addressNode, "./COUNTRY"),
                VATID = XmlUtils.nodeAsString(addressNode, "./VAT_ID"),
                Phone = XmlUtils.nodeAsString(addressNode, "./PHONE"), // @todo phone is typed
                Fax = XmlUtils.nodeAsString(addressNode, "./FAX") // @todo fax is typed
            };

            foreach(XmlNode contactDetailNode in addressNode.SelectNodes("./CONTACT_DETAILS"))
            {
                p.ContactDetails.Add(new ContactDetails()
                {
                    Id = XmlUtils.nodeAsString(contactDetailNode, "./CONTACT_ID"),
                    Name = XmlUtils.nodeAsString(contactDetailNode, "./CONTACT_NAME"),
                    FirstName = XmlUtils.nodeAsString(contactDetailNode, "./FIRST_NAME"),
                    Title = XmlUtils.nodeAsString(contactDetailNode, "./TITLE"),
                    AcademicTitle = XmlUtils.nodeAsString(contactDetailNode, "./ACADEMIC_TITLE"),
                    ContactRole = XmlUtils.nodeAsString(contactDetailNode, "./CONTACT_ROLE"), // @todo role is typed
                    ContactDescription = XmlUtils.nodeAsString(contactDetailNode, "./CONTACT_DESCR"),
                    Phone = XmlUtils.nodesAsStrings(contactDetailNode, "./PHONE"), // @todo phone is typed
                    Fax = XmlUtils.nodesAsStrings(contactDetailNode, "./FAX"), // @todo fax is typed
                    URL = XmlUtils.nodeAsString(contactDetailNode, "./URL"),
                    Emails = XmlUtils.nodeAsString(contactDetailNode, "./EMAILS") // @todo find out if this is a list in reality 
                });
            }

            return p;
        } // !_ReadyPartyAddress()
    }
}
