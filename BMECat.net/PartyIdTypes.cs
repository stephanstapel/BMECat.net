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
    public enum PartyIdTypes
    {
        BuyerSpecific,
        SupplierSpecific,
        DUNS,
        GLN
    }


    internal static class PartyIdTypesExtensions
    {
        public static PartyIdTypes? FromString(this PartyIdTypes _, string s)
        {
            s = s.ToLower().Trim();
            switch (s)
            {
                case "buyer_specific": return PartyIdTypes.BuyerSpecific;
                case "duns": return PartyIdTypes.DUNS;
                case "gln": return PartyIdTypes.GLN;
                case "supplier_specific": return PartyIdTypes.SupplierSpecific;

                default: return null;
            }
        } // !FromString()


        public static string EnumToString(this PartyIdTypes c)
        {
            switch (c)
            {
                case PartyIdTypes.BuyerSpecific: return "buyer_specific";
                case PartyIdTypes.DUNS: return "duns";
                case PartyIdTypes.GLN: return "gln";
                case PartyIdTypes.SupplierSpecific: return "supplier_specific";

                default: return "";
            }
        } // !ToString()
    }
}
