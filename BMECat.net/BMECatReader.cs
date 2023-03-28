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
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.XPath;
using static System.Net.Mime.MediaTypeNames;

namespace BMECat.net
{
    internal class BMECatReader
    {
        public static ProductCatalog Load(Stream stream, BMECatExtensions extensions = null)
        {
            Task<ProductCatalog> t = LoadAsync(stream, extensions);
            t.Wait();
            return t.Result;
        } // !Load()

        public static ProductCatalog Load(string filename, BMECatExtensions extensions = null)
        {
            Task<ProductCatalog> t = LoadAsync(filename, extensions);
            t.Wait();
            return t.Result;
        } // !Load()


        public async static Task<ProductCatalog> LoadAsync(Stream stream, BMECatExtensions extensions = null)
        {
            if (!stream.CanRead)
            {
                throw new IllegalStreamException("Cannot read from stream");
            }

            // quickly retrieve the document version
            BMECatVersion version = BMECatVersion.Version12;

            byte[] buffer = new byte[1024];
            stream.Read(buffer, 0, 1024);
            stream.Position = 0;

            string xmlHeading = System.Text.Encoding.UTF8.GetString(buffer);
            Regex regex = new Regex("<BMECAT[^>]+version=\"([^\"]+)\"");
            Match match = regex.Match(xmlHeading);
            if (match.Success && match.Groups.Count > 1)
            {
                if (match.Groups[1].Value.Contains("2005"))
                {
                    version = BMECatVersion.Version2005;
                }
            }
            
            switch (version)
            {
                case BMECatVersion.Version2005: return await BMECatReader2005.LoadAsync(stream, extensions);
                case BMECatVersion.Version12: return await BMECatReader12.LoadAsync(stream, extensions);
                default: throw new Exception($"Version {version} is currently not supported");
            }
        } // !LoadAsync()


        public static Task<ProductCatalog> LoadAsync(string filename, BMECatExtensions extensions = null)
        {
            if (!System.IO.File.Exists(filename))
            {
                throw new FileNotFoundException();
            }

            return LoadAsync(new FileStream(filename, FileMode.Open, FileAccess.Read), extensions);
        } // !LoadAsync()
    }
}
