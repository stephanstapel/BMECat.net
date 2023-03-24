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

namespace BMECat.net
{
    public class ContactDetails
    {
        public string Id { get; internal set; }
        public string Name { get; internal set; }
        public string FirstName { get; internal set; }
        public string Title { get; internal set; }
        public string AcademicTitle { get; internal set; }
        public string ContactRole { get; internal set; }
        public string ContactDescription { get; internal set; }        
        public List<string> Phone { get; internal set; } = new List<string>();
        public List<string> Fax { get; internal set; } = new List<string>();
        public string URL { get; internal set; }
        public string Emails { get; internal set; }
    }
}
