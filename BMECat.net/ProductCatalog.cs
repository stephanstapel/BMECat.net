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
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;

namespace BMECat.net
{
    public class ProductCatalog
    {
        /// <summary>
        /// Information zum Ersteller (manuell oder automatisch) des Dokuments
        /// </summary>
        public string GeneratorInfo { get; set; }
        public List<LanguageCodes> Languages { get; set; }

        /// <summary>
        /// Eindeutiger Identifikator des Kataloges; dieser wird normalerweise vom Lieferanten bei der
        /// ersten Katalogerstellung vergeben und verändert sich über den gesamten Lebenszyklus
        /// des Kataloges nicht
        /// </summary>
        public string CatalogId { get; set; }

        /// <summary>
        /// Version des Kataloges; darf nur bei Transaktion T_NEW_CATALOG im Zielsystem neu
        /// gesetzt werden, nicht aber bei Updates; siehe auch "Beispiel (Zusammenspiel verschiedener
        /// Transaktionen)"
        /// Format: “MajorVersion“.“MinorVersion“ (maximal jedoch xxx.yyy)
        /// Beispiel
        /// 001.120
        /// 7.3
        /// </summary>
        public string CatalogVersion { get; set; }

        /// <summary>
        /// Name des Kataloges
        /// Bsp.: Herbst/Winter 2005/2006
        /// </summary>
        public string CatalogName { get; set; }

        /// <summary>
        /// Zeitstempel für die Generierung des Katalogdokumentes
        /// </summary>
        public DateTime? GenerationDate { get; set; }
        public Party Buyer { get; set; }
        public Party Supplier { get; set; }
        public TransportConditions Transport { get; set; }
        public List<Product> Products { get; set; }
        public CurrencyCodes Currency { get; set; }
        public Agreement Agreement { get; set; }
        public List<CatalogStructure> CatalogStructures { get; set; } = new List<CatalogStructure>();



        public ProductCatalog()
        {
            this.Languages = new List<LanguageCodes>();
            this.Products = new List<Product>();
        } // !ProductCatalog()


        /// <summary>
        /// Saves the descriptor object into a stream.
        /// 
        /// The stream position will be reset to the original position after writing is finished.
        /// This allows easy further processing of the stream.
        /// </summary>
        /// <param name="stream"></param>
        public async Task SaveAsync(Stream stream, BMECatExtensions extensions = null)
        {
            BMECatWriter writer = new BMECatWriter();
            await writer.SaveAsync(this, stream, extensions);
        } // !Save()


        public async Task SaveAsync(string filename, BMECatExtensions extensions = null)
        {
            BMECatWriter writer = new BMECatWriter();
            await writer.SaveAsync(this, filename, extensions);
        } // !Save()


        public async static Task<ProductCatalog> LoadAsync(Stream stream, BMECatExtensions extensions = null)
        {
            return await BMECatReader.LoadAsync(stream, extensions);
        } // !Load()


        public async static Task<ProductCatalog> LoadAsync(string filename, BMECatExtensions extensions = null)
        {
            return await BMECatReader.LoadAsync(filename, extensions);
        } // !Load()


        public static ProductCatalog Load(Stream stream, BMECatExtensions extensions = null)
        {
            Task<ProductCatalog> t = LoadAsync(stream, extensions);
            t.Wait();
            return t.Result;
        } // !Load()


        public static ProductCatalog Load(string filename, BMECatExtensions extensions = null)
        {
            Task<ProductCatalog> t = LoadAsync(filename, extensions);
            t.Wait();
            return t.Result;
        } // !Load()


        /// <summary>
        /// Saves the descriptor object into a stream.
        /// 
        /// The stream position will be reset to the original position after writing is finished.
        /// This allows easy further processing of the stream.
        /// </summary>
        /// <param name="stream"></param>
        public void Save(Stream stream, BMECatExtensions extensions = null)
        {            
            Task t = SaveAsync(stream, extensions);
            t.Wait();
        } // !Save()


        public void Save(string filename, BMECatExtensions extensions = null)
        {            
            Task t = SaveAsync(filename, extensions);
            t.Wait();
        } // !Save()
    }
}
