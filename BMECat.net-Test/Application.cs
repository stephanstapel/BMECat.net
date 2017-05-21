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
using BMECat.net;

namespace BMECat.net_Test
{
    internal class Application
    {
        internal void run()
        {
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
        }
    }
}