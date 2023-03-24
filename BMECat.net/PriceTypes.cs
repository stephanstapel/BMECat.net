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
using System;
using System.Collections.Generic;
using System.Text;

namespace BMECat.net
{
    public enum PriceTypes
    {
        Unknown = 0,
        NetList,
        NetCustomer,
        NRP
    }



    internal static class PriceTypesExtensions
    {
        public static PriceTypes FromString(this PriceTypes _, string s)
        {
            s = s.ToLower().Trim();
            switch (s)
            {
                case "net_list": return PriceTypes.NetList;
                case "net_customer": return PriceTypes.NetCustomer;
                case "nrp": return PriceTypes.NRP;

                default: return PriceTypes.Unknown;
            }
        } // !FromString()


        public static string EnumToString(this PriceTypes c)
        {
            switch (c)
            {
                case PriceTypes.NetList: return "net_list";
                case PriceTypes.NetCustomer: return "net_customer";
                case PriceTypes.NRP: return "nrp";

                default: return "";
            }
        } // !ToString()
    }
}
