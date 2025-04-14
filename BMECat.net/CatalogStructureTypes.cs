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
    public enum CatalogStructureTypes
    {
        Leaf,
        Node
    }


    internal static class CatalogStructureTypesExtensions
    {
        public static CatalogStructureTypes? FromString(this CatalogStructureTypes _, string s)
        {
            s = s.ToLower().Trim();
            switch (s)
            {
                case "leaf": return CatalogStructureTypes.Leaf;
                case "node": return CatalogStructureTypes.Node;
                default: return null;
            }
        } // !FromString()


        public static string EnumToString(this CatalogStructureTypes c)
        {
            switch (c)
            {
                case CatalogStructureTypes.Leaf: return "leaf";
                case CatalogStructureTypes.Node: return "node";
                default: return "";
            }
        } // !ToString()
    }
}
