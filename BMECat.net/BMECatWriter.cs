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
            Writer.WriteAttributeString("xmlns", "http://www.bmecat.org/bmecat/2005fd");
            #endregion // !XML-Kopfbereich

            #region Header
            Writer.WriteStartElement("HEADER");
            _writeOptionalElementString(Writer, "GENERATOR_INFO", this.Catalog.GeneratorInfo);

            Writer.WriteStartElement("CATALOG");
            foreach (LanguageCodes _language in this.Catalog.Languages)
            {
                Writer.WriteElementString("LANGUAGE", _language.EnumToString());
            }
            Writer.WriteElementString("CATALOG_ID", this.Catalog.CatalogId); // Pflichtfeld
            Writer.WriteElementString("CATALOG_VERSION", this.Catalog.CatalogVersion); // Pflichtfeld
            _writeOptionalElementString(Writer, "CATALOG_NAME", this.Catalog.CatalogName);
            _writeDateTime(elementName: "GENERATION_DATE", date: this.Catalog.GenerationDate);
            Writer.WriteElementString("CURRENCY", this.Catalog.Currency.EnumToString());
            _writeTransport(Writer, this.Catalog.Transport);
            Writer.WriteEndElement(); // !CATALOG

            if (this.Catalog.Buyer != null)
            {
                Writer.WriteStartElement("BUYER");
                if (!String.IsNullOrEmpty(this.Catalog.Buyer.Id))
                {
                    Writer.WriteStartElement("BUYER_ID");
                    Writer.WriteAttributeString("type", "buyer_specific");
                    Writer.WriteString(this.Catalog.Buyer.Id);
                    Writer.WriteEndElement(); // !BUYER_ID
                }
                _writeOptionalElementString(Writer, "BUYER_NAME", this.Catalog.Buyer.Name);

                Writer.WriteStartElement("ADDRESS");
                Writer.WriteAttributeString("type", "buyer");
                Writer.WriteElementString("NAME", this.Catalog.Buyer.Name);
                if (!String.IsNullOrEmpty(this.Catalog.Buyer.ContactName))
                {
                    Writer.WriteElementString("CONTACT", this.Catalog.Buyer.ContactName);
                }
                Writer.WriteEndElement(); // !ADDRESS

                Writer.WriteEndElement(); // !BUYER
            }

            Writer.WriteEndElement(); // !HEADER
            #endregion // !Header

            #region PRODUCTS
            Writer.WriteStartElement("T_NEW_CATALOG");
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
                    _writeOptionalElementString(Writer, "INTERNATIONAL_PID", id.Id, new Dictionary<string, string>() { { "type", id.Type.ToString() } });
                }

                if (product.PIds.FirstOrDefault(p => p.Type.Equals(ProductIdTypes.EAN)) != null)
                {
                    
                }
                
                _writeOptionalElementString(Writer, "STOCK", String.Format("{0}", product.Stock));
                Writer.WriteEndElement(); // !PRODUCT_DETAILS

                if (product.OrderDetails != null)
                {
                    Writer.WriteStartElement("PRODUCT_ORDER_DETAILS");
                    _writeOptionalElementString(Writer, "ORDER_UNIT", product.OrderDetails.OrderUnit);
                    _writeOptionalElementString(Writer, "CONTENT_UNIT", product.OrderDetails.ContentUnit);
                    Writer.WriteEndElement(); // !PRODUCT_ORDER_DETAILS
                }

                if ((product.Prices != null) && (product.Prices.Count > 0))
                {
                    Writer.WriteStartElement("PRODUCT_PRICE_DETAILS");

                    foreach (ProductPrice price in product.Prices)
                    {
                        Writer.WriteStartElement("PRODUCT_PRICE");
                        Writer.WriteAttributeString("price_type", price.PriceType.ToString());
                        Writer.WriteElementString("PRICE_AMOUNT", price.Amount.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture));
                        Writer.WriteElementString("PRICE_CURRENCY", price.Currency.ToString());
                        Writer.WriteElementString("TAX", price.Tax.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture));
                        Writer.WriteElementString("LOWER_BOUND", price.Tax.ToString());
                        Writer.WriteEndElement(); // !PRODUCT_PRICE
                    }
                    Writer.WriteEndElement(); // !PRODUCT_PRICE_DETAILS
                }

                Writer.WriteEndElement(); // !PRODUCT
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
            FileStream fs = new FileStream(filename, FileMode.Create, FileAccess.Write);
            await SaveAsync(catalog, fs, extensions);
            fs.Flush();
            fs.Close();
        } // !SaveAsync()


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
            /*
            Writer.WriteElementString("DATE", date.ToString("yyyy-dd-MM"));
            Writer.WriteElementString("TIME", date.ToString("hh:mm"));
            Writer.WriteElementString("TIMEZONE", date.ToString("zzz"));
            */
                if (date.HasValue)
            {
                Writer.WriteString(date.Value.ToString("yyyy-MM-ddThh:mm:sszzz"));
            }
            Writer.WriteEndElement();
        } // !_writeDateTime()


        private string _formatDecimal(double value, int numDecimals = 2)
        {
            return _formatDecimal((decimal)value, numDecimals);
        } // !_formatDecimal()


        private string _formatDecimal(float value, int numDecimals = 2)
        {
            return _formatDecimal((decimal)value, numDecimals);
        } // !_formatDecimal()


        private string _formatDecimal(decimal value, int numDecimals = 2)
        {
            string formatString = "0.";
            for (int i = 0; i < numDecimals; i++)
            {
                formatString += "0";
            }

            return value.ToString(formatString).Replace(",", ".");
        } // !_formatDecimal()


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
        } // !_writeOptionalElementString


        private void _writeOptionalElementString(XmlTextWriter writer, string tagName, QuantityCode value, BMECatExtensions extensions = null)
        {
            if (value.ClearText != null)
            {
                writer.WriteElementString(tagName, value.ClearText);
            }
            else if (value.Code != QuantityCodes.Unknown)
            {
                if ((extensions != null) && (extensions.QuantityCodeConverter != null))
                {
                    writer.WriteElementString(tagName, extensions.QuantityCodeConverter.Convert(value.Code));
                }
                else
                {
                    writer.WriteElementString(tagName, value.Code.ToString());
                }
            }
        } // !_writeOptionalElementString()
    }
}