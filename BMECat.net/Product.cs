﻿/*
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
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Xml.Linq;

namespace BMECat.net
{
    public class Product
    {
        public string No { get; set; }
        public List<ProductId> PIds { get; set; } = new List<ProductId>();
        public string DescriptionShort { get; set; }
        public string DescriptionLong { get; set; }        
        public int? Stock { get; set; }

        public List<string> Keywords { get; set; } = new List<string>();

        public Product()
        {
        }
        

        /// <summary>
        /// Description of the product by features and/or classification of the product
        /// </summary>
        public List<FeatureSet> FeatureSets { get; set; } = new List<FeatureSet>();

        public List<ProductId> SupplierPIds { get; set; } = new List<ProductId>();
        public OrderDetails OrderDetails { get; set; }
        public List<ProductPrice> Prices { get; set; } = new List<ProductPrice>();
        public List<MimeInfo> MimeInfos { get; set; } = new List<MimeInfo>();
        public LogisticsDetails LogisticsDetails { get; set; }
        public string SupplierAltPid { get; set; }

        /// <summary>
        /// Product ID of the manufacturer
        /// </summary>
        public string ManufacturerPID { get; set; }

        /// <summary>
        /// This element contains the name of the manufacturer of the product.
        /// </summary>
        public string ManufacturerName { get; set; }

        /// <summary>
        /// The manufacturer’s type description is a name for the product which may, in certain
        /// circumstances, be more widely-known than the correct product name.When a
        /// manufacturer’s type description is specified, the name of the manufacturer must also
        /// be specified (MANUFACTURER_NAME).
        /// </summary>
        public string ManufacturerTypeDescription { get; set; }

        /// <summary>
        /// Specifices the material group or material class of the article in the supplier’s ERP
        /// system
        /// </summary>
        public string ERPGroupSupplier { get; set; }

        /// <summary>
        /// Specifies the material group or material class of the article in the ERP system of the
        /// buying company
        /// Value range: Depends on buying firm’s ERP(BUYER)
        /// </summary>
        public string ERPGroupBuyer { get; set; }
        public List<Reference> References { get; set; } = new List<Reference>();
        public EDXF EDXF { get; set; }
        public List<Tuple<string, string>> ExtendedInformation { get; set; } = new List<Tuple<string, string>>();
        public List<ProductCatalogGroupMapping> ProductCatalogGroupMappings { get; set; } = new List<ProductCatalogGroupMapping>();



        public Feature GetFeature(string featureName, bool ignoreCase = true, Feature defaultValue = null)
        {
            foreach (FeatureSet featureSet in this.FeatureSets)
            {
                if (featureSet.Contains(featureName, ignoreCase))
                {
                    return featureSet.Get(featureName, ignoreCase);
                }
            }

            return defaultValue;
        } // !GetFeature()


        public void RemoveFeature(string featureName, bool ignoreCase = true)
        {
            foreach (FeatureSet featureSet in this.FeatureSets)
            {
                if (ignoreCase)
                {
                    featureSet.Features = featureSet.Features.Where(f => !f.Name.ToLower().Equals(featureName.ToLower())).ToList();
                }
                else
                {
                    featureSet.Features = featureSet.Features.Where(f => !f.Name.Equals(featureName)).ToList();
                }                
            }
        } // !RemoveFeature()
    }
}