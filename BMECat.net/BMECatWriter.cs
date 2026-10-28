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
    public class BMECatWriter
    {
        /// <summary>
        /// Namespace of the BMECat 2005 release
        /// </summary>
        public const string Namespace = "http://www.bmecat.org/bmecat/2005";

        public ProductCatalog Catalog { get; private set; }
        private XmlTextWriter Writer { get; set; }


        public async Task SaveAsync(ProductCatalog catalog, Stream stream, BMECatExtensions extensions = null)
        {
            if (!stream.CanWrite || !stream.CanSeek)
            {
                throw new IllegalStreamException("Cannot write to stream");
            }

            long streamPosition = stream.Position;

            this.Catalog = catalog;
            this.Writer = new XmlTextWriter(stream, Encoding.UTF8);
            Writer.Formatting = Formatting.Indented;
            Writer.WriteStartDocument();

            #region XML-Kopfbereich
            Writer.WriteStartElement("BMECAT");
            Writer.WriteAttributeString("version", "2005");
            Writer.WriteAttributeString("xmlns", Namespace);
            #endregion // !XML-Kopfbereich

            #region Header
            Writer.WriteStartElement("HEADER");
            _writeOptionalElementString(Writer, "GENERATOR_INFO", this.Catalog.GeneratorInfo);

            Writer.WriteStartElement("CATALOG");
            foreach (LanguageCodes language in this.Catalog.Languages)
            {
                Writer.WriteElementString("LANGUAGE", language.EnumToString());
            }
            Writer.WriteElementString("CATALOG_ID", this.Catalog.CatalogId); // Pflichtfeld
            Writer.WriteElementString("CATALOG_VERSION", this.Catalog.CatalogVersion); // Pflichtfeld
            _writeOptionalElementString(Writer, "CATALOG_NAME", this.Catalog.CatalogName);
            _writeDateTime(elementName: "GENERATION_DATE", date: this.Catalog.GenerationDate);
            Writer.WriteElementString("CURRENCY", this.Catalog.Currency.EnumToString());
            _writeTransport(Writer, this.Catalog.Transport);
            Writer.WriteEndElement(); // !CATALOG

            // BMECat 2005 describes buyer and supplier as parties, the header only references them
            if (this.Catalog.Buyer != null)
            {
                _writeOptionalElementString(Writer, "BUYER_IDREF", _partyId(this.Catalog.Buyer, "buyer"), new Dictionary<string, string>() { { "type", "buyer_specific" } });
            }

            if (this.Catalog.Supplier != null)
            {
                _writeOptionalElementString(Writer, "SUPPLIER_IDREF", _partyId(this.Catalog.Supplier, "supplier"), new Dictionary<string, string>() { { "type", "supplier_specific" } });
            }
            else
            {
                // SUPPLIER_IDREF is mandatory, but there is no party it could reference
                extensions?.LogService?.Warn("The catalog has no supplier, the written document does not conform to the BMECat schema");
            }

            if ((this.Catalog.Buyer != null) || (this.Catalog.Supplier != null))
            {
                Writer.WriteStartElement("PARTIES");
                _writeParty(this.Catalog.Buyer, "buyer", extensions);
                _writeParty(this.Catalog.Supplier, "supplier", extensions);
                Writer.WriteEndElement(); // !PARTIES
            }

            Writer.WriteEndElement(); // !HEADER
            #endregion // !Header

            #region PRODUCTS
            Writer.WriteStartElement("T_NEW_CATALOG");
            _writeCatalogGroupSystem(this.Catalog.CatalogStructures);

            foreach(Product product in this.Catalog.Products)
            {
                Writer.WriteStartElement("PRODUCT");
                Writer.WriteAttributeString("mode", "new");
                _writeOptionalElementString(Writer, "SUPPLIER_PID", product.No);

                Writer.WriteStartElement("PRODUCT_DETAILS");
                _writeOptionalElementString(Writer, "DESCRIPTION_SHORT", product.DescriptionShort);
                _writeOptionalElementString(Writer, "DESCRIPTION_LONG", product.DescriptionLong);

                foreach(ProductId id in product.PIds)
                {
                    _writeOptionalElementString(Writer, "INTERNATIONAL_PID", id.Id, new Dictionary<string, string>() { { "type", id.Type.EnumToString() } });
                }


                
                _writeOptionalElementString(Writer, "MANUFACTURER_PID", String.Format("{0}", product.ManufacturerPID));
                _writeOptionalElementString(Writer, "MANUFACTURER_NAME", String.Format("{0}", product.ManufacturerName));
                _writeOptionalElementString(Writer, "MANUFACTURER_TYPE_DESCR", String.Format("{0}", product.ManufacturerTypeDescription));
                _writeOptionalElementString(Writer, "ERP_GROUP_BUYER", String.Format("{0}", product.ERPGroupBuyer));
                _writeOptionalElementString(Writer, "ERP_GROUP_SUPPLIER", String.Format("{0}", product.ERPGroupSupplier));

                foreach (string keyword in product.Keywords ?? new List<string>())
                {
                    _writeOptionalElementString(Writer, "KEYWORD", keyword);
                }

                Writer.WriteEndElement(); // !PRODUCT_DETAILS

                foreach (FeatureSet featureSet in product.FeatureSets ?? new List<FeatureSet>())
                {
                    _writeFeatureSet(featureSet, extensions);
                }

                if (product.OrderDetails != null)
                {
                    Writer.WriteStartElement("PRODUCT_ORDER_DETAILS");
                    if (!_writeOptionalElementString(Writer, "ORDER_UNIT", product.OrderDetails.OrderUnit, extensions))
                    {
                        // ORDER_UNIT is mandatory within PRODUCT_ORDER_DETAILS. There is no sensible default unit,
                        // hence the remaining order details are still written instead of being dropped silently
                        extensions?.LogService?.Warn($"Product {product.No} has order details without order unit, the written document does not conform to the BMECat schema");
                    }
                    _writeOptionalElementString(Writer, "CONTENT_UNIT", product.OrderDetails.ContentUnit, extensions);
                    _writeOptionalElementString(Writer, "NO_CU_PER_OU", _formatInteger(product.OrderDetails.ContentUnitPerOrderUnit));
                    _writeOptionalElementString(Writer, "PRICE_QUANTITY", _formatInteger(product.OrderDetails.PriceQuantity));
                    _writeOptionalElementString(Writer, "QUANTITY_MIN", _formatInteger(product.OrderDetails.QuantityMin));
                    _writeOptionalElementString(Writer, "QUANTITY_INTERVAL", _formatInteger(product.OrderDetails.QuantityInterval));
                    Writer.WriteEndElement(); // !PRODUCT_ORDER_DETAILS
                }

                if ((product.Prices != null) && (product.Prices.Count > 0))
                {
                    Writer.WriteStartElement("PRODUCT_PRICE_DETAILS");

                    foreach (ProductPrice price in product.Prices)
                    {
                        Writer.WriteStartElement("PRODUCT_PRICE");
                        Writer.WriteAttributeString("price_type", price.PriceType.EnumToString());
                        Writer.WriteElementString("PRICE_AMOUNT", price.Amount.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture));
                        Writer.WriteElementString("PRICE_CURRENCY", price.Currency.ToString());
                        Writer.WriteElementString("TAX", price.Tax.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture));
                        _writeOptionalElementString(Writer, "LOWER_BOUND", _formatInteger(price.LowerBound));
                        Writer.WriteEndElement(); // !PRODUCT_PRICE
                    }
                    Writer.WriteEndElement(); // !PRODUCT_PRICE_DETAILS
                }

                _writeMimeInfos(product.MimeInfos);

                foreach (Reference reference in product.References ?? new List<Reference>())
                {
                    if (String.IsNullOrEmpty(reference.IdTo))
                    {
                        continue;
                    }

                    if (reference.Type == ReferenceTypes.Unknown)
                    {
                        // the type attribute is mandatory, a reference of unknown type cannot be written
                        extensions?.LogService?.Warn($"Product {product.No} references {reference.IdTo} without known type, the reference is not written");
                        continue;
                    }

                    Writer.WriteStartElement("PRODUCT_REFERENCE");
                    Writer.WriteAttributeString("type", reference.Type.EnumToString());
                    Writer.WriteElementString("PROD_ID_TO", reference.IdTo);
                    Writer.WriteEndElement(); // !PRODUCT_REFERENCE
                }

                _writeLogisticsDetails(product.LogisticsDetails);

                Writer.WriteEndElement(); // !PRODUCT
            }

            foreach (Product product in this.Catalog.Products)
            {
                foreach (ProductCatalogGroupMapping mapping in product.ProductCatalogGroupMappings ?? new List<ProductCatalogGroupMapping>())
                {
                    // both ids are mandatory, a mapping without them would reference nothing
                    if (String.IsNullOrEmpty(product.No) || String.IsNullOrEmpty(mapping?.CatalogGroupId))
                    {
                        continue;
                    }

                    Writer.WriteStartElement("PRODUCT_TO_CATALOGGROUP_MAP");
                    Writer.WriteAttributeString("mode", "new");
                    Writer.WriteElementString("PROD_ID", product.No);
                    Writer.WriteElementString("CATALOG_GROUP_ID", mapping.CatalogGroupId);
                    _writeOptionalElementString(Writer, "PRODUCT_TO_CATALOGGROUP_MAP_ORDER", _formatInteger(mapping.Order));
                    Writer.WriteEndElement(); // !PRODUCT_TO_CATALOGGROUP_MAP
                }
            }
            Writer.WriteEndElement(); // !T_NEW_CATALOG
            #endregion // !ARTICLES


            Writer.WriteEndElement(); // !BMECAT
            Writer.WriteEndDocument();
            Writer.Flush();

            stream.Seek(streamPosition, SeekOrigin.Begin);

            await Task.CompletedTask;
        } // !SaveAsync()


        public async Task SaveAsync(ProductCatalog catalog, string filename, BMECatExtensions extensions = null)
        {
            using (FileStream fs = new FileStream(filename, FileMode.Create, FileAccess.Write))
            {
                await SaveAsync(catalog, fs, extensions);
                fs.Flush();
            }
        } // !SaveAsync()


        /// <summary>
        /// The header references the parties by their id, parties without id are identified by their role
        /// </summary>
        private static string _partyId(Party party, string role)
        {
            return String.IsNullOrEmpty(party.Id) ? role : party.Id;
        } // !_partyId()


        private void _writeParty(Party party, string role, BMECatExtensions extensions)
        {
            if (party == null)
            {
                return;
            }

            string partyId = _partyId(party, role);
            Writer.WriteStartElement("PARTY");
            _writeOptionalElementString(Writer, "PARTY_ID", partyId, new Dictionary<string, string>() { { "type", $"{role}_specific" } });
            Writer.WriteElementString("PARTY_ROLE", role);

            Writer.WriteStartElement("ADDRESS");
            _writeOptionalElementString(Writer, "NAME", party.Name);
            _writeOptionalElementString(Writer, "NAME2", party.Name2);
            _writeOptionalElementString(Writer, "NAME3", party.Name3);
            _writeOptionalElementString(Writer, "DEPARTMENT", party.Department);

            // CONTACT and CONTACT_DETAILS are alternatives, the structured contact details take precedence
            List<ContactDetails> contacts = party.ContactDetails ?? new List<ContactDetails>();
            if (contacts.Count == 0)
            {
                _writeOptionalElementString(Writer, "CONTACT", party.ContactName);
            }
            else if (!String.IsNullOrEmpty(party.ContactName))
            {
                extensions?.LogService?.Warn($"Party {partyId} has a contact name and contact details, only the contact details are written");
            }

            for (int i = 0; i < contacts.Count; i++)
            {
                ContactDetails contact = contacts[i];
                Writer.WriteStartElement("CONTACT_DETAILS");

                // CONTACT_ID is mandatory, contacts without id are numbered within their party
                Writer.WriteElementString("CONTACT_ID", String.IsNullOrEmpty(contact.Id) ? $"{partyId}-{i + 1}" : contact.Id);
                _writeOptionalElementString(Writer, "CONTACT_NAME", contact.Name);
                _writeOptionalElementString(Writer, "FIRST_NAME", contact.FirstName);
                _writeOptionalElementString(Writer, "TITLE", contact.Title);
                _writeOptionalElementString(Writer, "ACADEMIC_TITLE", contact.AcademicTitle);
                _writeOptionalElementString(Writer, "CONTACT_ROLE", contact.ContactRole);
                _writeOptionalElementString(Writer, "CONTACT_DESCR", contact.ContactDescription);
                foreach (string phone in contact.Phone ?? new List<string>())
                {
                    _writeOptionalElementString(Writer, "PHONE", phone);
                }
                foreach (string fax in contact.Fax ?? new List<string>())
                {
                    _writeOptionalElementString(Writer, "FAX", fax);
                }
                _writeOptionalElementString(Writer, "URL", contact.URL);
                if (!String.IsNullOrEmpty(contact.Emails))
                {
                    Writer.WriteStartElement("EMAILS");
                    Writer.WriteElementString("EMAIL", contact.Emails);
                    Writer.WriteEndElement(); // !EMAILS
                }
                Writer.WriteEndElement(); // !CONTACT_DETAILS
            }

            _writeOptionalElementString(Writer, "STREET", party.Street);
            _writeOptionalElementString(Writer, "ZIP", party.Zip);
            _writeOptionalElementString(Writer, "BOXNO", party.BoxNo);
            _writeOptionalElementString(Writer, "ZIPBOX", party.ZipBox);
            _writeOptionalElementString(Writer, "CITY", party.City);
            _writeOptionalElementString(Writer, "STATE", party.State);
            _writeOptionalElementString(Writer, "COUNTRY", party.Country);
            _writeOptionalElementString(Writer, "VAT_ID", party.VATID);
            _writeOptionalElementString(Writer, "PHONE", party.Phone);
            _writeOptionalElementString(Writer, "FAX", party.Fax);
            Writer.WriteEndElement(); // !ADDRESS

            Writer.WriteEndElement(); // !PARTY
        } // !_writeParty()


        private void _writeFeatureSet(FeatureSet featureSet, BMECatExtensions extensions)
        {
            Writer.WriteStartElement("PRODUCT_FEATURES");

            FeatureClassificationSystem classification = featureSet.FeatureClassificationSystem;
            if (classification != null)
            {
                _writeOptionalElementString(Writer, "REFERENCE_FEATURE_SYSTEM_NAME", classification.Classification);

                // REFERENCE_FEATURE_GROUP_ID and REFERENCE_FEATURE_GROUP_NAME are alternatives, the ids take precedence
                List<string> groupIds = (classification.GroupIds ?? new List<FeatureClassificationSystemGroupId>()).Select(g => g?.Name)
                                                                                                                 .Where(name => !String.IsNullOrEmpty(name))
                                                                                                                 .ToList();
                foreach (string groupId in groupIds)
                {
                    Writer.WriteElementString("REFERENCE_FEATURE_GROUP_ID", groupId);
                }

                if (groupIds.Count == 0)
                {
                    _writeOptionalElementString(Writer, "REFERENCE_FEATURE_GROUP_NAME", classification.GroupName);
                }
                else if (!String.IsNullOrEmpty(classification.GroupName))
                {
                    extensions?.LogService?.Warn($"Feature system {classification.Classification} has group ids and a group name, only the group ids are written");
                }
            }

            foreach (Feature feature in featureSet.Features ?? new List<Feature>())
            {
                Writer.WriteStartElement("FEATURE");
                Writer.WriteElementString("FNAME", feature.Name ?? "");
                foreach (string value in feature.Values ?? new List<string>())
                {
                    Writer.WriteElementString("FVALUE", value ?? "");
                }
                _writeOptionalElementString(Writer, "FUNIT", feature.Unit, extensions);
                _writeOptionalElementString(Writer, "FORDER", feature.Order);
                _writeOptionalElementString(Writer, "FDESCR", feature.Description);
                Writer.WriteEndElement(); // !FEATURE
            }

            Writer.WriteEndElement(); // !PRODUCT_FEATURES
        } // !_writeFeatureSet()


        private void _writeMimeInfos(List<MimeInfo> mimeInfos)
        {
            if ((mimeInfos == null) || (mimeInfos.Count == 0))
            {
                return;
            }

            Writer.WriteStartElement("MIME_INFO");
            foreach (MimeInfo mimeInfo in mimeInfos)
            {
                Writer.WriteStartElement("MIME");
                _writeOptionalElementString(Writer, "MIME_TYPE", mimeInfo.MimeType.EnumToString());
                Writer.WriteElementString("MIME_SOURCE", mimeInfo.Source ?? "");
                _writeOptionalElementString(Writer, "MIME_DESCR", mimeInfo.Description);
                _writeOptionalElementString(Writer, "MIME_ALT", mimeInfo.Alt);
                _writeOptionalElementString(Writer, "MIME_PURPOSE", mimeInfo.Purpose);
                _writeOptionalElementString(Writer, "MIME_ORDER", _formatInteger(mimeInfo.Order));
                Writer.WriteEndElement(); // !MIME
            }
            Writer.WriteEndElement(); // !MIME_INFO
        } // !_writeMimeInfos()


        private void _writeLogisticsDetails(LogisticsDetails logisticsDetails)
        {
            if (logisticsDetails == null)
            {
                return;
            }

            Writer.WriteStartElement("PRODUCT_LOGISTIC_DETAILS");
            foreach (string customsTariffNumber in logisticsDetails.CustomsTariffNumber ?? new List<string>())
            {
                if (String.IsNullOrEmpty(customsTariffNumber))
                {
                    continue;
                }

                Writer.WriteStartElement("CUSTOMS_TARIFF_NUMBER");
                Writer.WriteElementString("CUSTOMS_NUMBER", customsTariffNumber);
                Writer.WriteEndElement(); // !CUSTOMS_TARIFF_NUMBER
            }

            _writeOptionalElementString(Writer, "COUNTRY_OF_ORIGIN", logisticsDetails.CountryOfOrigin?.EnumToString());

            if (logisticsDetails.Volume.HasValue || logisticsDetails.Weight.HasValue || logisticsDetails.Length.HasValue ||
                logisticsDetails.Width.HasValue || logisticsDetails.Depth.HasValue)
            {
                Writer.WriteStartElement("PRODUCT_DIMENSIONS");
                _writeOptionalElementString(Writer, "VOLUME", _formatDecimal(logisticsDetails.Volume));
                _writeOptionalElementString(Writer, "WEIGHT", _formatDecimal(logisticsDetails.Weight));
                _writeOptionalElementString(Writer, "LENGTH", _formatDecimal(logisticsDetails.Length));
                _writeOptionalElementString(Writer, "WIDTH", _formatDecimal(logisticsDetails.Width));
                _writeOptionalElementString(Writer, "DEPTH", _formatDecimal(logisticsDetails.Depth));
                Writer.WriteEndElement(); // !PRODUCT_DIMENSIONS
            }
            Writer.WriteEndElement(); // !PRODUCT_LOGISTIC_DETAILS
        } // !_writeLogisticsDetails()


        private void _writeCatalogGroupSystem(List<CatalogStructure> catalogStructures)
        {
            if ((catalogStructures == null) || (catalogStructures.Count == 0))
            {
                return;
            }

            Writer.WriteStartElement("CATALOG_GROUP_SYSTEM");
            foreach (CatalogStructure catalogStructure in catalogStructures)
            {
                Writer.WriteStartElement("CATALOG_STRUCTURE");
                if (catalogStructure.Type.HasValue)
                {
                    Writer.WriteAttributeString("type", catalogStructure.Type.Value.EnumToString());
                }
                Writer.WriteElementString("GROUP_ID", catalogStructure.GroupId ?? "");
                Writer.WriteElementString("GROUP_NAME", catalogStructure.GroupName ?? "");
                Writer.WriteElementString("PARENT_ID", catalogStructure.ParentId ?? ""); // mandatory, also for the root
                _writeOptionalElementString(Writer, "GROUP_ORDER", catalogStructure.GroupOrder);
                _writeMimeInfos(catalogStructure.MimeInfos);
                Writer.WriteEndElement(); // !CATALOG_STRUCTURE
            }
            Writer.WriteEndElement(); // !CATALOG_GROUP_SYSTEM
        } // !_writeCatalogGroupSystem()


        private static string _formatDecimal(decimal? value)
        {
            return value.HasValue ? value.Value.ToString(System.Globalization.CultureInfo.InvariantCulture) : null;
        } // !_formatDecimal()


        private void _writeTransport(XmlTextWriter writer, TransportConditions transportCondition)
        {
            if (transportCondition == null)
            {
                return;
            }

            writer.WriteStartElement("TRANSPORT");
            writer.WriteElementString("INCOTERM", transportCondition.Incoterm.EnumToString());
            _writeOptionalElementString(Writer, "LOCATION", transportCondition.Location);
            _writeOptionalElementString(Writer, "TRANSPORT_REMARK", transportCondition.Remark);
            writer.WriteEndElement(); // !TRANSPORT
        } // !_writeTransport()


        private void _writeDateTime(string elementName, string typeAttribute = "", DateTime? date = null)
        {
            Writer.WriteStartElement(elementName);
            if (!string.IsNullOrEmpty(typeAttribute))
            {
                Writer.WriteAttributeString("type", typeAttribute);
            }

            if (date.HasValue)
            {
                // HH: 24 hour format, invariant culture: gregorian calendar and latin digits, independent from the machine the code runs on
                Writer.WriteString(date.Value.ToString("yyyy-MM-ddTHH:mm:sszzz", System.Globalization.CultureInfo.InvariantCulture));
            }
            Writer.WriteEndElement();
        } // !_writeDateTime()


        private void _writeOptionalElementString(XmlTextWriter writer, string tagName, string value, Dictionary<string, string> attributes = null)
        {
            if (!String.IsNullOrEmpty(value))
            {
                writer.WriteStartElement(tagName);
                if (attributes != null)
                {
                    foreach(KeyValuePair<string, string> attr in attributes)
                    {
                        writer.WriteAttributeString(attr.Key, attr.Value);
                    }
                }
                writer.WriteValue(value);
                writer.WriteEndElement();                
            }
        } // !_writeOptionalElementString()


        private static string _formatInteger(int? value)
        {
            return value.HasValue ? value.Value.ToString(System.Globalization.CultureInfo.InvariantCulture) : null;
        } // !_formatInteger()


        /// <returns>true if the element was written</returns>
        private bool _writeOptionalElementString(XmlTextWriter writer, string tagName, QuantityCode value, BMECatExtensions extensions = null)
        {
            if (value == null)
            {
                return false;
            }

            if (!String.IsNullOrEmpty(value.ClearText))
            {
                writer.WriteElementString(tagName, value.ClearText);
                return true;
            }

            if (!value.Code.HasValue || (value.Code == QuantityCodes.Unknown))
            {
                return false;
            }

            // the code itself is used if the converter does not know the code
            string text = extensions?.QuantityCodeConverter?.Convert(value.Code);
            if (String.IsNullOrEmpty(text))
            {
                text = value.Code.Value.EnumToString();
            }

            writer.WriteElementString(tagName, text);
            return true;
        } // !_writeOptionalElementString()
    }
}
