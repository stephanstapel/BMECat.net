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
    public enum ReferenceTypes
    {
        Unknown,
        SparePart,
        Accessories,
        ConsistsOf,
        Similar,
        Select,
        Mandatory,
        FollowUp,
        BaseProduct,
        Others
    }



    internal static class ReferenceTypesExtensions
    {
        public static ReferenceTypes FromString(this ReferenceTypes _, string s)
        {
            s = s.ToLower().Trim();
            switch (s)
            {
                case "sparepart": return ReferenceTypes.SparePart;
                case "others": return ReferenceTypes.Others;
                case "accessories": return ReferenceTypes.Accessories;
                case "consists_of": return ReferenceTypes.ConsistsOf;
                case "similar": return ReferenceTypes.Similar;
                case "select": return ReferenceTypes.Select;
                case "mandatory": return ReferenceTypes.Mandatory;
                case "followup": return ReferenceTypes.FollowUp;
                case "base_product": return ReferenceTypes.BaseProduct;
                default:
                    {
                        Console.WriteLine($"Unknown reference type {s}");
                        return ReferenceTypes.Unknown;
                    }
            }
        } // !FromString()


        public static string EnumToString(this ReferenceTypes c)
        {
            switch (c)
            {
                case ReferenceTypes.SparePart: return "sparepart";
                case ReferenceTypes.Others: return "others";
                case ReferenceTypes.Accessories: return "accessories";
                case ReferenceTypes.ConsistsOf: return "consists_of";
                case ReferenceTypes.Similar: return "similar";
                case ReferenceTypes.Select:  return "select";
                case ReferenceTypes.Mandatory: return "mandatory";
                case ReferenceTypes.FollowUp: return "followup";
                case ReferenceTypes.BaseProduct: return "base_product";
                default: return "";
            }
        } // !ToString()
    }
}
