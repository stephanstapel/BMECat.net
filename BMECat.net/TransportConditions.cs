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
namespace BMECat.net
{
    public class TransportConditions
    {
        public IncotermCodes Incoterm { get; set; }
        /// <summary>
        /// Übergang der Ware von Anbieter zu Nachfrager oder umgekehrt. Abhängig von INCOTERM.
        /// 
        /// EXW: insert named place of delivery
        /// DAT: insert named terminal at port or place of destination
        /// FCA: insert named place of delivery
        /// DAP: insert named place of destination
        /// CPT: insert named place of destination
        /// DDP: insert named place of destination
        /// CIP: insert named place of destination
        /// FAS: insert named port of shipment
        /// CFR: insert named port of destination
        /// FOB: insert named port of shipment
        /// CIF: insert named port of destination
        /// </summary>
        public string Location { get; set; }

        /// <summary>
        /// Bemerkung für die Transportart
        /// </summary>
        public string Remark { get; set; }
    }
}