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

namespace BMECat.net
{ 
    public enum LanguageCodes
    {
        /// <summary>
        /// Deutsch
        /// </summary>
        DEU,

        /// <summary>
        /// Englisch
        /// </summary>
        ENG
    }


    public static class LanguageCodesExtensions
    {
        public static LanguageCodes? FromString(this LanguageCodes _c, string s)
        {
            if (Enum.TryParse(s, true, out LanguageCodes result))
            {
                return result;
            }

            return null;
        } // !FromString()


        public static string EnumToString(this LanguageCodes c)
        {
            return c.ToString("g").ToLower();
        } // !ToString()
    }
}
