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
using System.Collections.Generic;

namespace BMECat.net
{
    public class Party
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string ContactName { get; set; }

        public List<ContactDetails> ContactDetails { get; set; } = new List<ContactDetails>();
        public string Name2 { get; internal set; }
        public string Name3 { get; internal set; }
        public string Street { get; internal set; }
        public string Zip { get; internal set; }
        public string BoxNo { get; internal set; }
        public string ZipBox { get; internal set; }
        public string City { get; internal set; }
        public string State { get; internal set; }
        public string Country { get; internal set; }
        public string VATID { get; internal set; }
        public string Phone { get; internal set; }
        public string Fax { get; internal set; }
        public string Department { get; internal set; }
    }
}