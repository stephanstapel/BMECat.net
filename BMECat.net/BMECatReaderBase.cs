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
using System.IO;
using System.Threading.Tasks;

namespace BMECat.net
{
    internal class BMECatReaderBase
    {        
        protected static QuantityCode _convertQuantityCode(string input, BMECatExtensions extensions)
        {
            if (input == null)
            {
                return new QuantityCode();
            }
            if ((extensions != null) && (extensions.QuantityCodeConverter != null))
            {
                return new QuantityCode() { ClearText = input, Code = extensions.QuantityCodeConverter.Convert(input) };
            }
            else
            {
                return new QuantityCode() { ClearText = input, Code = default(QuantityCodes).FromString(input) };
            }
        } // !_convertQuantityCode()
    }
}