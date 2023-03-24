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
using System.Runtime.InteropServices;
using System.Text;

namespace BMECat.net
{       
    public class OrderDetails
    {
        /// <summary>
        /// Bestelleinheit
        /// 
        /// Einheit, in der das Produkte bestellt werden kann; es können nur Vielfache dieser Einheit
        /// bestellt werden.
        /// Auf diese Einheit (oder auf Teile oder auf Vielfache davon) bezieht sich stets auch der
        /// Preis.
        /// Beispiel: Kiste Mineralwasser mit 6 Flaschen
        /// Bestelleinheit: "Kiste", Inhaltseinheit/Einheit des Artikels: "Flasche"
        /// Verpackungsmenge: "6"
        /// </summary>
        public QuantityCode OrderUnit { get; set; } = new QuantityCode();

        /// <summary>
        /// Inhaltseinheit
        /// 
        /// Einheit des Produktes innerhalb einer Bestelleinheit
        /// </summary>
        public QuantityCode ContentUnit { get; set; } = new QuantityCode();
        public int? ContentUnitPerOrderUnit { get; set; }
        public int? PriceQuantity { get; set; }
        public int? QuantityMin { get; set; }
        public int? QuantityInterval { get; set;}
    }
}
