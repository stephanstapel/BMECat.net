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
using System.Text;
using System.Xml.Linq;

namespace BMECat.net
{
    public class FeatureClassificationSystem
    {
        /// <summary>
        /// Name of the referenced classification or feature system
        /// </summary>
        public string Classification { get; set; }

        /// <summary>
        /// This element contains a reference to the unique identifier of an existing group of the respective classification system
        /// The group can also be referenced by its unique, though language-dependent name.
        /// </summary>
        public List<FeatureClassificationSystemGroupId> GroupIds { get; set; } = new List<FeatureClassificationSystemGroupId>();

        /// <summary>
        /// Reference to the unique, though language-dependent name of an existing group of the respective         
        /// classification system. This element may only be used if the GroupId element is not used.
        /// </summary>
        public string GroupName { get; set; }
    }
}
