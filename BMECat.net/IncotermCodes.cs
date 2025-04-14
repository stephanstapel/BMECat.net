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

namespace BMECat.net
{
    /// <summary>
    /// https://www.unece.org/fileadmin/DAM/cefact/recommendations/rec14/ece_trd_c_cf_2011_5E_Rec5.pdf
    /// </summary>
    public enum IncotermCodes
    {
        /// <summary>
        /// EX WORKS 
        /// </summary>
        EXW,

        /// <summary>
        /// DELIVERED AT TERMINAL
        /// </summary>
        DAT,

        /// <summary>
        /// FREE CARRIER 
        /// </summary>
        FCA,

        /// <summary>
        /// DELIVERED AT PLACE 
        /// </summary>
        DAP,

        /// <summary>
        /// CARRIAGE PAID TO 
        /// </summary>
        CPT,

        /// <summary>
        /// DELIVERED DUTY PAID 
        /// </summary>
        DDP,

        /// <summary>
        /// CARRIAGE AND INSURANCE PAID TO 
        /// </summary>
        CIP,

        /// <summary>
        /// FREE ALONGSIDE SHIP
        /// </summary>
        FAS,

        /// <summary>
        /// COST AND FREIGHT 
        /// </summary>
        CFR,

        /// <summary>
        /// FREE ON BOARD
        /// </summary>
        FOB,

        /// <summary>
        /// COST, INSURANCE AND FREIGHT 
        /// </summary>
        CIF
    }



    public static class IncotermCodesExtensions
    {
        public static IncotermCodes? FromString(this IncotermCodes _c, string s)
        {
            if (Enum.TryParse(s, true, out IncotermCodes result))
            {
                return result;
            }

            return null;            
        } // !FromString()


        public static string EnumToString(this IncotermCodes? c)
        {
            if (!c.HasValue)
            {
                return "";
            }

            return c.Value.ToString("g");
        } // !ToString()
    }
}
