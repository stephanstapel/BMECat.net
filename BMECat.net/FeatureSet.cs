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
using System.Linq;
using System.Text;

namespace BMECat.net
{
    public class FeatureSet
    {
        public FeatureClassificationSystem FeatureClassificationSystem { get; set; }
        public IList<Feature> Features { get; internal set; } = new List<Feature>();


        public bool Contains(string featureName, bool ignoreCase = true)
        {
            if (ignoreCase)
            {
                Feature featureFound = this.Features.FirstOrDefault(f => f.Name.ToLower().Equals(featureName.ToLower()));
                return (featureFound != null);
            }
            else
            {
                Feature featureFound = this.Features.FirstOrDefault(f => f.Name.Equals(featureName));
                return (featureFound != null);
            }
        } // !Contains()


        public Feature Get(string featureName, bool ignoreCase = true)
        {
            if (ignoreCase)
            {
                return this.Features.FirstOrDefault(f => f.Name.ToLower().Equals(featureName.ToLower()));                
            }
            else
            {
                return this.Features.FirstOrDefault(f => f.Name.Equals(featureName));
            }
        } // !Contains()
    }
}
